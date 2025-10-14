# Verification

## Обзор
Внешний Assembly Definition: **GameFramework.Verification**.

Define: **VERIFY_WINDOW_ENABLED**

Назначение:
- Проверяет корректность значений.
- Отлавливает ошибки конфигурации.
- Валидирует игровые объекты

### Логика работы
- Verify принимает condition и возвращает bool.
  - Если condition == true → вернуть true
  - Если condition == false → вернуть false. Записать сообщение в консоль. Если определен дефайн **VERIFICATION_WINDOW_ENABLED** (см [Defines](../)) и в этой сессии не был нажат **Skip All** - показать окно.
- Исключений не бросает.

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
- объект-контекст (если есть).

## Публичное API
```csharp
bool Verify(bool condition, string? message = null, UnityEngine.Object? context = null);
bool Verify(System.Func<bool> condition, string? message = null, UnityEngine.Object? context = null);
```

## Использование
### Через Inspector
```csharp
using GameFramework.Verification;
using UnityEngine;

public class FooClass : MonoBehaviour
{
    void FooFunction()
    {
        Inspector.Verify(condition: someFlag, message: "My message");
    }

    void FooFunction2()
    {
        Inspector.Verify(() => SomeExpensiveCheck(), "My message");
    }

    bool SomeExpensiveCheck() => true;
}
```
### Атрибут Verifiable
Атрибут добавляет в класс методы Verify(...) через генерацию кода (см. [Генерация кода](../../Core/SourceGeneration/README.md)).
```csharp
using GameFramework.Verification;
using UnityEngine;

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

## Best Practice
### Простая проверка объектов
**UnityEngine.Object** неявно приводится к bool (obj != null).

```csharp
class FooClass : MonoBehaviour
{
    GameObject someGameObject;

    void Init()
    {
        Verify(someGameObject, "SomeGameObject is missing");
        // ...
    }
}
```
### Ранний выход
Снижает шум и защищает от побочных эффектов.

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
## Производительность
- Используйте лямбда-версию, когда проверка дорогая.
- Делегат вычисляется один раз внутри Verify