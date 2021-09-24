using System;

//mehhhhhhh, I don't like this

//basically intended usage is something like:
//
// [SerializeField, PEnumType(typeof(DamageType))]
// private EditorPEnum TargetDamageType;
//
// and then editor scripts make it actually work
// rereading documentation on PropertyAttribute and PropertyDrawer, though
//
// I wonder if something like this is possible?
// [SerializeField, PEnumType(typeof(DamageType))]
// private DamageType TargetDamageType;
//
// or even this:
// [SerializeField, PEnumType(typeof(DamageType))]
// private int TargetDamageType;
//
// in the latter cases, might want to change the attribute to something like PEnumEditor

[Serializable]
public class EditorPEnum
{
    public string UninterpretedValue;

    public object InterpretedValue; //will be set by editor script

    public T InterpretAs<T>() where T : Enum
    {
        return (T)PEnum.ParseToEnum(typeof(T), UninterpretedValue, true);
    }
}

public class PEnumTypeAttribute : Attribute
{
    public Type TargetType { get; private set; }

    public PEnumTypeAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}

//see: https://answers.unity.com/questions/929293/get-field-type-of-serializedproperty.html when we get to actually building the inspector components