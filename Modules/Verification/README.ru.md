# Verification

## Обзор
- Проверяет корректность значений.
- Отлавливает ошибки конфигурации.
- Валидирует игровые объекты

### Логика работы
- Verify принимает condition и возвращает bool - результат condition. Дальнешие действия в зависимости от реализации контекста.
  - DebugVerificationContext - если не был определен дефайн BUILD_PRODUCTION
    - Если condition == true → вернуть true
    - Если condition == false → вернуть false. Записать сообщение в консоль. Показать окно (если в этой сессии не был нажат **Skip All**)
  - ProductionVerificationContext - если был определен дефайн BUILD_PRODUCTION
    - Если condition == true → вернуть true
    - Если condition == false → вернуть false. Записать сообщение в консоль. Отправить аналитический ивент
  - TestVerificationContext - используется в тестах
    - Если condition == true → вернуть true
    - Если condition == false → вернуть false
- Исключений не бросает

### Окно
Сообщает о причине провала верификации.

Кнопки:

|     **Ok**      |                                                 **Skip All**                                                 |
|:---------------:|:------------------------------------------------------------------------------------------------------------:|
| Закрывает окно  | Закрывает окно и больше не показывает его в течение текущей сессии. Ошибки продолжают отправляться в консоль |

Содержит:
- путь до файла.
- название файла.
- строку в которой производилась верификация.

## Публичное API
```csharp
bool Verify(bool condition, string? message = null, UnityEngine.Object? context = null);
bool Verify(System.Func<bool> condition, string? message = null, UnityEngine.Object? context = null);
```

## Использование
### Атрибут Verifiable (рекомендуется)
Атрибут добавляет в класс методы Verify(...) через генерацию кода (см. [Генерация кода](../../Core/SourceGeneration/README.md)).
```csharp
using UnityEngine;
using GameFramework.Verification;

[Verifiable]
public partial class FooClass : MonoBehaviour
{
    void FooFunction()
    {
        Verify(someFlag, "My message");
    }

    void FooFunction2()
    {
        Verify(() => someFlag && SomeCheck(), "Another message");
    }

    bool SomeCheck() => true;
}
```

### VerificationContainer.Context
```csharp
using UnityEngine;
using GameFramework.Verification;

public class FooClass : MonoBehaviour
{
    void FooFunction()
    {
        VerificationContainer.Context.Verify(condition: someFlag, message: "My message");
    }

    void FooFunction2()
    {
        VerificationContainer.Context.Verify(() => SomeExpensiveCheck(), "My message");
    }

    bool SomeExpensiveCheck() => true;
}
```

## Best Practice
### Ранний выход
Снижает шум, защищает от невалидных/неверных значений

```csharp
class FooClass : MonoBehaviour
{
    GameObject someGameObject;
    Config someConfig;

    void Enable()
    {
        if (!Verify(someGameObject, "Missing SomeGameObject")) 
        {
            return; 
        }
        someGameObject.SetActive(true);
        // ...
    }

    void Compute()
    {
        if (!Verify(someConfig.value != 0, "Config.value must be non-zero")) 
        {
            return;
        }
        // ...
    }
}
```
### Простая проверка объектов
**UnityEngine.Object** неявно приводится к bool (obj != null).
```csharp
class FooClass : MonoBehaviour
{
    GameObject someGameObject;
    GameObject someGameObject2;

    void Init()
    {
        if(!Verify(someGameObject, "SomeGameObject is missing"))
        {
            return;
        }
        // ...
    }
    
    void PostInit()
    {
        Verify(someGameObject2, "SomeGameObject is missing");
        // ...
    }
}
```

## Производительность
- Используйте лямбда-версию, когда проверка дорогая.
- Делегат вычисляется один раз внутри Verify