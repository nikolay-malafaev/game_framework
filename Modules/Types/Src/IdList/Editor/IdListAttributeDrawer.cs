using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public struct IdOption
{
    public string Path;
    public string Value;
}

public class StringIdSelector : OdinSelector<string>
{
    private readonly List<IdOption> _options;

    public StringIdSelector(List<IdOption> options)
    {
        _options = options;
    }

    protected override void BuildSelectionTree(OdinMenuTree tree)
    {
        tree.Config.DrawSearchToolbar = true;
        if (_options != null)
        {
            foreach (var opt in _options)
            {
                tree.Add(opt.Path, opt.Value);
            }
        }
    }
}

public interface IIdListResolver
{
    int Priority { get; }
    bool CanHandle(string listName);
    void Initialize(IdListAttribute attribute, InspectorProperty property);
    string GetErrorMessage();
    void UpdateOptions(List<IdOption> options, HashSet<string> validValues);
}

public class TypeIdListResolver : IIdListResolver
{
    public int Priority => 10;
    private string _errorMessage;
    private List<IIdList> _idLists = new List<IIdList>();
    private List<string> _listNames = new List<string>();
    private string _targetTypeName;

    public bool CanHandle(string listName) => listName.StartsWith("t:");

    public void Initialize(IdListAttribute attribute, InspectorProperty property)
    {
        string listNameStr = attribute.ListName;
        _targetTypeName = listNameStr.Substring(2);
        var guids = AssetDatabase.FindAssets(listNameStr);
        
        List<ScriptableObject> matchingSOs = new List<ScriptableObject>();
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so != null)
            {
                matchingSOs.Add(so);
            }
        }

        if (matchingSOs.Count == 0)
        {
            _errorMessage = $"Not found any ScriptableObject matching '{listNameStr}'.";
        }
        else
        {
            foreach (var so in matchingSOs)
            {
                if (so is IIdList idList)
                {
                    _idLists.Add(idList);
                    string groupName = so.name;
                    if (groupName.EndsWith(_targetTypeName))
                    {
                        groupName = groupName.Substring(0, groupName.Length - _targetTypeName.Length);
                    }
                    _listNames.Add(groupName);
                }
            }
            if (_idLists.Count == 0 && string.IsNullOrEmpty(_errorMessage))
            {
                _errorMessage = $"Found objects for '{listNameStr}', but none implement IIdList.";
            }
        }
    }

    public string GetErrorMessage() => _errorMessage;

    public void UpdateOptions(List<IdOption> options, HashSet<string> validValues)
    {
        for (int i = 0; i < _idLists.Count; i++)
        {
            var ids = _idLists[i].GetIds();
            string groupName = _listNames[i];
            
            if (ids == null) continue;
            
            foreach (var id in ids)
            {
                if (id == null) continue;
                string safeId = string.IsNullOrEmpty(id) ? "<Empty>" : id;
                string path = !string.IsNullOrEmpty(groupName) ? $"{groupName}/{safeId}" : safeId;
                options.Add(new IdOption { Path = path, Value = id });
                validValues.Add(id);
            }
        }
    }
}

public class NameIdListResolver : IIdListResolver
{
    public int Priority => 0; // Fallback
    private string _errorMessage;
    private IIdList _idList;

    public bool CanHandle(string listName) => true;

    public void Initialize(IdListAttribute attribute, InspectorProperty property)
    {
        string listNameStr = attribute.ListName;
        var guids = AssetDatabase.FindAssets($"t:ScriptableObject {listNameStr}");
        
        List<ScriptableObject> matchingSOs = new List<ScriptableObject>();
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so != null && so.name == listNameStr)
            {
                matchingSOs.Add(so);
            }
        }

        if (matchingSOs.Count == 0)
        {
            _errorMessage = $"Not found any ScriptableObject matching '{listNameStr}'.";
        }
        else if (matchingSOs.Count > 1)
        {
            _errorMessage = $"Found more than one ScriptableObject with name '{listNameStr}'.";
        }
        else
        {
            if (matchingSOs[0] is IIdList idList)
            {
                _idList = idList;
            }
            else
            {
                _errorMessage = $"ScriptableObject '{listNameStr}' does not implement IIdList.";
            }
        }
    }

    public string GetErrorMessage() => _errorMessage;

    public void UpdateOptions(List<IdOption> options, HashSet<string> validValues)
    {
        if (_idList == null) return;
        var ids = _idList.GetIds();
        if (ids == null) return;
        
        foreach (var id in ids)
        {
            if (id == null) continue;
            string safeId = string.IsNullOrEmpty(id) ? "<Empty>" : id;
            options.Add(new IdOption { Path = safeId, Value = id });
            validValues.Add(id);
        }
    }
}

public class FunctionIdListResolver : IIdListResolver
{
    public int Priority => 10;
    private string _errorMessage;
    private string _functionName;
    private MethodInfo _methodInfo;
    private object _targetObject;

    public bool CanHandle(string listName) => listName.StartsWith("f:");

    public void Initialize(IdListAttribute attribute, InspectorProperty property)
    {
        _functionName = attribute.ListName.Substring(2);
        
        if (property.ParentValues.Count > 0)
        {
            _targetObject = property.ParentValues[0];
        }
        
        if (_targetObject == null)
        {
            _errorMessage = $"Target object is null for function '{_functionName}'.";
            return;
        }

        Type targetType = _targetObject.GetType();
        _methodInfo = targetType.GetMethod(_functionName, 
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, 
            null, Type.EmptyTypes, null);
        
        if (_methodInfo == null)
        {
            _errorMessage = $"Method '{_functionName}()' not found in type '{targetType.Name}'.";
            return;
        }

        if (!typeof(IEnumerable<string>).IsAssignableFrom(_methodInfo.ReturnType))
        {
            _errorMessage = $"Method '{_functionName}()' must return IEnumerable<string>; actual return type is {_methodInfo.ReturnType.Name}.";
        }
    }

    public string GetErrorMessage() => _errorMessage;

    public void UpdateOptions(List<IdOption> options, HashSet<string> validValues)
    {
        if (_methodInfo == null) return;

        object result = _methodInfo.Invoke(_methodInfo.IsStatic ? null : _targetObject, null);
        if (result is IEnumerable<string> strArray)
        {
            foreach (var id in strArray)
            {
                if (id == null) continue;
                string safeId = string.IsNullOrEmpty(id) ? "<Empty>" : id;
                options.Add(new IdOption { Path = safeId, Value = id });
                validValues.Add(id);
            }
        }
    }
}

public class KeyedIdListResolver : IIdListResolver
{
    public int Priority => 10;
    private string _errorMessage;
    private List<GameFramework.StaticData.KeyedStaticDataAsset> _assets = new List<GameFramework.StaticData.KeyedStaticDataAsset>();
    private string _targetTypeName;

    public bool CanHandle(string listName) => listName.StartsWith("k:");

    public void Initialize(IdListAttribute attribute, InspectorProperty property)
    {
        string listNameStr = attribute.ListName;
        _targetTypeName = listNameStr.Substring(2);
        
        var guids = AssetDatabase.FindAssets($"t:{_targetTypeName}");
        
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so is GameFramework.StaticData.KeyedStaticDataAsset keyedAsset)
            {
                _assets.Add(keyedAsset);
            }
        }

        if (_assets.Count == 0 && guids.Length > 0)
        {
            _errorMessage = $"Found objects for type '{_targetTypeName}', but they do not inherit from KeyedStaticDataAsset.";
        }
        else if (_assets.Count == 0)
        {
            _errorMessage = $"Not found any assets matching type '{_targetTypeName}'.";
        }
    }

    public string GetErrorMessage() => _errorMessage;

    public void UpdateOptions(List<IdOption> options, HashSet<string> validValues)
    {
        foreach (var asset in _assets)
        {
            if (asset == null) continue;
            string key = asset.Key;
            
            if (string.IsNullOrEmpty(key)) continue;
            
            options.Add(new IdOption { Path = key, Value = key });
            validValues.Add(key);
        }
    }
}

public class DynamicIdListResolver : IIdListResolver
{
    public int Priority => 100;
    private string _errorMessage;
    private string _functionName;
    private MethodInfo _methodInfo;
    private object _targetObject;
    private InspectorProperty _property;
    
    private string _currentDynamicListName;
    private IIdListResolver _currentResolver;

    public bool CanHandle(string listName) => listName.StartsWith("dynamic-list-name:");

    public void Initialize(IdListAttribute attribute, InspectorProperty property)
    {
        _property = property;
        _functionName = attribute.ListName.Substring("dynamic-list-name:".Length);
        
        if (property.ParentValues.Count > 0)
        {
            _targetObject = property.ParentValues[0];
        }
        
        if (_targetObject == null)
        {
            _errorMessage = $"Target object is null for dynamic function '{_functionName}'.";
            return;
        }

        Type targetType = _targetObject.GetType();
        _methodInfo = targetType.GetMethod(_functionName, 
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, 
            null, Type.EmptyTypes, null);
        
        if (_methodInfo == null)
        {
            _errorMessage = $"Method '{_functionName}()' not found in type '{targetType.Name}'.";
            return;
        }

        if (_methodInfo.ReturnType != typeof(string))
        {
            _errorMessage = $"Method '{_functionName}()' must return string; actual return type is {_methodInfo.ReturnType.Name}.";
        }
        
        UpdateDynamicResolver();
    }
    
    private void UpdateDynamicResolver()
    {
        if (_methodInfo == null) return;
        
        string newListName = _methodInfo.Invoke(_methodInfo.IsStatic ? null : _targetObject, null) as string;
        
        if (newListName != _currentDynamicListName)
        {
            _currentDynamicListName = newListName;
            
            if (string.IsNullOrEmpty(_currentDynamicListName))
            {
                _currentResolver = null;
                return;
            }
            
            var resolvers = IdListAttributeDrawer.CreateResolvers();
            _currentResolver = resolvers.FirstOrDefault(r => r.CanHandle(_currentDynamicListName));
            
            if (_currentResolver != null)
            {
                var dummyAttribute = new IdListAttribute(_currentDynamicListName);
                _currentResolver.Initialize(dummyAttribute, _property);
            }
        }
    }

    public string GetErrorMessage() 
    {
        if (!string.IsNullOrEmpty(_errorMessage)) return _errorMessage;
        if (string.IsNullOrEmpty(_currentDynamicListName)) return "Dynamic list name is empty.";
        if (_currentResolver != null) return _currentResolver.GetErrorMessage();
        return $"Cannot resolve dynamic list name '{_currentDynamicListName}'.";
    }

    public void UpdateOptions(List<IdOption> options, HashSet<string> validValues)
    {
        UpdateDynamicResolver();
        
        if (_currentResolver != null)
        {
            _currentResolver.UpdateOptions(options, validValues);
        }
    }
}

public class IdListAttributeDrawer : OdinAttributeDrawer<IdListAttribute, string>
{
    private static List<Type> _stringResolvers;
    private IIdListResolver _resolver;
    
    private List<IdOption> _options = new List<IdOption>();
    private HashSet<string> _validValues = new HashSet<string>();
    private double _lastUpdateTime;

    public static List<IIdListResolver> CreateResolvers()
    {
        if (_stringResolvers == null)
        {
            _stringResolvers = TypeCache.GetTypesDerivedFrom<IIdListResolver>()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .ToList();
        }

        var resolvers = new List<IIdListResolver>();
        foreach (var type in _stringResolvers)
        {
            resolvers.Add((IIdListResolver)Activator.CreateInstance(type));
        }
        
        resolvers.Sort((a, b) => b.Priority.CompareTo(a.Priority)); // Highest priority first
        return resolvers;
    }

    protected override void Initialize()
    {
        var resolvers = CreateResolvers();

        string listName = Attribute.ListName;
        _resolver = resolvers.FirstOrDefault(r => r.CanHandle(listName));
        
        if (_resolver != null)
        {
            _resolver.Initialize(Attribute, Property);
            ForceUpdateOptions();
        }
    }

    private void TryUpdateOptions()
    {
        if (EditorApplication.timeSinceStartup - _lastUpdateTime > 1.0)
        {
            ForceUpdateOptions();
        }
    }

    private void ForceUpdateOptions()
    {
        _options.Clear();
        _validValues.Clear();
        
        if (_resolver != null)
        {
            _resolver.UpdateOptions(_options, _validValues);
        }
        
        _lastUpdateTime = EditorApplication.timeSinceStartup;
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (_resolver == null)
        {
            SirenixEditorGUI.ErrorMessageBox($"No valid resolver found for '{Attribute.ListName}'");
            CallNextDrawer(label);
            return;
        }

        string error = _resolver.GetErrorMessage();
        if (!string.IsNullOrEmpty(error))
        {
            SirenixEditorGUI.ErrorMessageBox(error);
            CallNextDrawer(label);
            return;
        }

        TryUpdateOptions();

        if (ValueEntry.SmartValue == null || !_validValues.Contains(ValueEntry.SmartValue))
        {
            SirenixEditorGUI.ErrorMessageBox($"Value '{ValueEntry.SmartValue}' is not found in the {Attribute.ListName} list!");
        }

        Rect rect = SirenixEditorGUI.GetFeatureRichControlRect(label, out int controlId, out bool hasFocus, out Rect valueRect);
        
        string displayValue = string.IsNullOrEmpty(ValueEntry.SmartValue) ? "None" : ValueEntry.SmartValue;
        
        if (GUI.Button(valueRect, new GUIContent(displayValue), EditorStyles.popup))
        {
            ForceUpdateOptions(); // just in case
            
            StringIdSelector selector = new StringIdSelector(_options);
            selector.SetSelection(ValueEntry.SmartValue);
            
            selector.SelectionChanged += x =>
            {
                if (x != null && x.Any())
                {
                    ValueEntry.SmartValue = x.FirstOrDefault();
                    ValueEntry.ApplyChanges();
                }
            };
            
            selector.SelectionConfirmed += x =>
            {
                if (x != null && x.Any())
                {
                    ValueEntry.SmartValue = x.FirstOrDefault();
                    ValueEntry.ApplyChanges();
                }
            };
            
            selector.EnableSingleClickToSelect();
            selector.ShowInPopup(valueRect);
        }
    }
}

[InitializeOnLoad]
public static class IIdListValidatorGUI
{
    static IIdListValidatorGUI()
    {
        Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
    }

    private static void OnPostHeaderGUI(Editor editor)
    {
        if (editor.target is IIdList idList)
        {
            var ids = idList.GetIds();
            if (ids == null) return;
            
            foreach (var id in ids)
            {
                if (!IsValidId(id))
                {
                    SirenixEditorGUI.ErrorMessageBox($"Invalid ID format found in list: '{id}'. Minimum 1 char. If 1 char, must be letter or digit.");
                }
            }
        }
    }

    private static bool IsValidId(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        if (id.Length == 1) return char.IsLetterOrDigit(id[0]);
        return true;
    }
}
