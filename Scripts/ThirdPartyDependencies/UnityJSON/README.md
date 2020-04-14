# UnityJSON

> Customizable JSON Serialization / Deserialization for C# in Unity

**Table Of Contents:**

* [Features](#features)
* [Installation](#installation)
* [Serialization](#serialization)
  - [Serialization Lifecycle](#serialization-lifecycle)
  - [Custom Serialization with Serializer](#custom-serialization-with-serializer)
  - [Custom Serialization with ISerializable](#custom-serialization-with-iserializable)
* [Deserialization](#deserialization)
  - [Deserialized Types](#deserialized-types)
  - [Deserialization of Extra Nodes](#deserialization-of-extra-nodes)
  - [Inheritance](#inheritance)
  - [Constructors](#constructors)
  - [Deserialization Lifecycle](#deserialization-lifecycle)
  - [Custom Deserialization with Instantiater](#custom-deserialization-with-instantiater)
  - [Custom Deserialization with Deserializer](#custom-deserialization-with-deserializer)
  - [Custom Deserialization with IDeserializable](#custom-deserialization-with-ideserializable)
* [Special Types](#special-types)
  - [Enums](#enums)
  - [Tuples](#tuples)
* [Changelog](#changelog)

## Features

UnityJSON provides direct JSON serialization / deserialization between
C# objects and JSON strings. Main features of UnityJSON are:

- Serializes all primitive types, enums, structs and classes into JSON 
string directly.
- Deserializes all primitive types, enums, most of the structs and classes 
from JSON string directly.
- Supports inheritence during deserialization.
- Supports formatting for enums.
- Supports formatting of serialized / deserialized fields and 
properties.
- Supports Unity types (Vectors, Quaternions, Color, Rect, Bounds).
- Provides further customization of serialization / deserialization process.

UnityJSON works with reflection using C# attributes. The following is a very
simple example using UnityJSON:

```cs
using UnityJSON;

public class ParentClass
{
    // Supports structs.
    public struct NestedStruct
    {
        // Supports Unity Vectors
        public Vector2 vectorField;
    }

    // Supports properties.
    public NestedStruct structProperty { get; set; }

    // Supports deserialization to interfaces.
    public IList listField;
}

// Deserialization
var parentObject = JSON.Deserialize<ParentClass>(
    "{\"structProperty\": {\"vectorField\": " +
    "{\"x\":1, \"y\": 2}}, \"listField\":[true]}");

// Serialization
parentObject.ToJSONString();
```

## Installation

Simply use the `unityjson.unitypackage` to add it to your project.

## Serialization

You can serialize objects with `object.ToJSONString()` or `JSON.Serialize(object)`
methods. By default, only public instance fields and properties of an object are
serialized, however, you can customize the serialized properties with the use
of `JSONNode` and `JSONObject` attributes. 

```cs
// Include statics in the serialization / deserialization.
[JSONObject(ObjectOptions.IncludeStatic)]
public class AClass
{
    // Serialized because of ObjectOptions.IncludeStatic.
    public static int staticIntField;

    // Serialize although it is a private field.
    [JSONNode]
    private static string staticStringField;

    // Don't serialize although it is a public property.
    [JSONNode(NodeOptions.DontSerialize)]
    public IDictionary dictionaryField;

    // Serialize this field even if it is null. When serializing
    // use the key "customField" instead of "stringField".
    [JSONNode(NodeOptions.SerializeNull, key = "customField")]
    private string stringField;
}
```

`JSONNodeAttribute`s are the main attributes for fields and properties, and define 
their serialization / deserialization configuration. The attribute contains options 
in the form of `NodeOptions` enum and an optional custom key for the field or property.
The node options only affect the field or property they are bound to with some minor
exceptions when using enumerable types. For serialization, the following node options 
might be useful:

- DontSerialize: Ignores the field / property during serialization. Can be used with
public fields / properties that should not be serialized.
- SerializeNull: By default if a value is null, it is simply ignored and not serialized
(except for enumerables other than dictionaries such as lists or arrays). This option
makes sure that the field / property is serialized anyway. When bound to a dictionary
(IDictionary, IDictionary<,>, ...), this option also affects the values of the dictionary.

`JSONObjectAttribute`s offer general control over the serialization / deserialization
process. They can be added to classes and structs to inform the serializer where to look
at. It uses the `ObjectOptions` enum which has the following serialization options:

- IgnoreProperties: Ignores all properties from serialization / deserialization.
- IncludeStatic: Includes static fields and properties in serialization / deserialization.
- TupleFormat: Handles the struct or the class as a tuple of fields. See [Tuples](#tuples)
for more detail.

### Serialization Lifecycle

You can listen to the serialization lifecycle of an object by implementing the
`ISerializationListener` interface.

```cs
public class AClass : ISerializationListener
{
    void ISerializationListener.OnSerializationWillBegin(Serializer serializer)
    {
        Debug.Log ("Serialization started.");
    }

    void ISerializationListener.OnSerializationSucceeded(Serializer serializer)
    {
        Debug.Log ("Serialization ended successfully.");
    }

    void ISerializationListener.OnSerializationFailed(Serializer serializer)
    {
        Debug.Log ("Serialization failed.");
    }
}
```

The `OnSerializationWillBegin` call is always followed by either the success or
fail call. The fail method is called just before throwing an exception.

### Custom Serialization with Serializer

Serializer is the actual component that performs the serialization. The
basic serializer can be accessed with `Serializer.Simple`. When no specific
serializer is given, the default serializer is used (`Serializer.Default`). The
default serializer is the simple serializer unless set otherwise. You can
create your own serializer by simply subclassing `Serializer`. You should then
override the `Serializer.TrySerialize` method to perform your application
specific serialization.

```cs
public class SpecialSerializer : Serializer
{
    protected override bool TrySerialize (
        object obj, 
        NodeOptions options, 
        out string serialized)
    {
        // obj is always guaranteed to be non-null.
        if (obj.GetType() == typeof(MySpecialClass)) {
            serialized = MySpecialSerializeFunction();
            return true;
        } else {
            // Returning false will simply run the regular
            // serialization.
            return false;
        }
    }
}

var obj = new MySpecialClass();
obj.ToJSONString(new SpecialSerializer());
```

You can then pass this new serializer as an argument in the `object.ToJSONString` 
method or set it as the default serializer to be used automatically. The classes that
are serialized with the `Serializer.TrySerialize` method do not receive serialization
lifecycle calls from `ISerializationListener`.

### Custom Serialization with ISerializable

Another way to provide custom serialization is by implementing the interface
`ISerializable`. It is important to notice that `Serializer.TrySerialize` is called
first and this interface will be ignored if that method returns true.

```cs
public class AClass : ISerializable
{
    string ISerializable.Serialize(Serializer serializer)
    {
        return "{" + serializer.SerializeEnum (MyEnum.Value) + "}";
    }
}
```

The classes that are serialized with the `ISerializable.Serialize` method do not receive 
serialization lifecycle calls from `ISerializationListener`.

## Deserialization

You can perform deserialization by calling `JSON.Deserialize<T>(jsonString)` method.
This will instantiate a new object of that type and fill it with the data from the
JSON string. If you wish to use a previously created object, you can also use one of the
`JSON.DeserializeOn(obj, jsonString)` or `obj.FeedJSON(jsonString)` methods. This object
can either be a struct or a class.

Deserialization can be more cumbersome then it's counterpart as the serialization of an
object is unique whereas the deserialization of a JSON string can produce different
results. The basic JSON types allow the following deserialization approaches:

- JSON string: System.String (string)
- JSON bool: System.Boolean (bool)
- JSON number: System.Int32 (int), System.UInt32 (uint), System.Byte (byte), 
System.Single (float), System.Double (double)
- JSON array: System.Array, IList, IList\<\>, List\<\>
- JSON dictionary: IDictionary, IDictionary\<,\>, Dictionary\<,\>, custom classes and structs

UnityJSON aims to support a lot of the main system classes such as lists and dictionaries.
However, not everything is supported directly by framework, for instance LinkedLists are
not supported for deserialization at the moment.

The framework decides the type to deserialize into from the type of the field or property.
This is important as the deserializer needs to instantiate an object from the given type
only. If an interface such as `IList` is used, then a default instantiated object type is
defined, `List<>` is used for instance for `IList`.

`JSONNode` and `JSONObject` attributes are also used for deserialization and provide
more options.

```cs
// If an unknown key is received during deserialization, simply
// ignore it instead of throwing an exception.
[JSONObject(ObjectOptions.IgnoreUnknownKey)]
public class AClass
{
    // Automatically ignored at deserialization as the property
    // does not have a setter.
    public float propertyField { get; }

    // Deserialize although it is a private field.
    [JSONNode]
    private string stringField;

    // Don't deserialize although it is a public property.
    [JSONNode(NodeOptions.DontDeserialize)]
    public IDictionary dictionaryField;

    // Don't throw an exception even if the deserialized object
    // doesn't match the type (Vector2 expects "x" and "y" keys).
    [JSONNode(NodeOptions.IgnoreTypeMismatch)]
    public Vector2 vectorField;

    // Don't throw an exception even if the deserializer cannot
    // instantiate an object for the type. This can for example
    // happen for classes without a default constructor.
    [JSONNode(NodeOptions.IgnoreInstantiationError)]
    public ClassWithConstructor classField;

    // Even if the JSON object has a "customField" key, don't
    // assign it if it is null, simply ignore it.
    [JSONNode(NodeOptions.DontAssignNull, key = "customField")]
    private string stringField2;
}
```

Deserialization process tends to throw a lot of `DeserializationException`s exceptions in 
case of any problems. By default, the exception is thrown in the following scenarios:

- The JSON string is not applicable to the target type. For instance the target type is
a string and the node contains a boolean value. This is called a type mismatch error. You
can ignore it with the `NodeOptions.IgoreTypeMismatch` option.
- The target type cannot be instantiated or is not supported. This is called an unknown
type error. You can ignore it with the `NodeOptions.IgnoreInstantiationError` option.
- The JSON node cannot contain unknown keys that cannot be mapped to the fields and properties
of the class / struct. In such a scenario an exception is thrown, this is called an
unknown key error and can be ignored with `ObjectOptions.IgnoreUnknownKey` option given
in a `JSONObjectAttribute` to the class or struct. Another option to ignore this error is
by using an extras dictionary (see [Deserialization of Extra Nodes](#deserialization-of-extra-nodes)).

`NodeOptions` also offer the following other options for the deserialization process:

- DontDeserialize: Ignore the field / property from the deserialization process. Similar
to `NodeOptions.DontSerialize` for serialization.
- DontAssignNull: By default if a key exists in the node its value is assigned to the
associated field or property automatically even if it is null. This option makes sure
that the null assignments are simply ignored by the deserializer. This can be particularly
useful to use with `NodeOptions.IgnoreTypeMismatch` because when the type mismatch errors
are ignored, in case of a type mismatch the deserializer always returns null. For
primitive types, null is mapped to the intricate default value (0 for int, false for bool).
By using this option, you can prevent the deserializer from assigning the default values
upon type mismatch errors.
- ReplaceDeserialized: The deserializer tries to reuse the previously created objects
when deserializing. If a field is of type T and it is instantiated, when the deserializer
recevies values for this object of type T, it simply assigns them directly to the already
existing object. This option forces the deserializer to instantiate a new object instead
completely build from the data recevied. This option can only be used for custom classes
that do not implement IEnumerable interface. An example can be seen below:

```cs
public class A
{
    public int intField;
    public float floatField;
}

public struct B
{
    public A a1 = new A();

    [JSONNode(NodeOptions.ReplaceDeserialized)]
    public A a2 = new A();
}

B b = new B();

// Without ReplaceDeserialized
b.a1.floatField = 2;
B deserializedB = JSON.Deserialize<B>("{\"a1:{\"intField\":1}}");
deserializedB.a1.intField // 1 (from the deserialization)
deserializedB.a1.floatField // 2 (from the previous assignment, object reused)

// With ReplaceDeserialized
b.a2.floatField = 2;
B deserializedB2 = JSON.Deserialize<B>("{\"a2:{\"intField\":1}}");
deserializedB2.a2.intField // 1 (from the deserialization)
deserializedB2.a2.floatField // 0 (new object of type A is instantiated)
```

### Deserialized Types

The deserialization is currently defined for the following target types:

- int, uint, byte, bool, float, double
- Enums (see [Enums](#enums) for fomatting details)
- T[]: Type T must be supported
- System.Array: Instantiates object[]
- List\<T\>: Type T must be supported
- IList\<T\>: Instantiates List\<T\>
- IList: Instantiates List\<object\>
- Dictionary\<K, V\>: K must be primitive, enum or string. V must be supported.
- IDictionary\<K, V\>: Instantiates Dictionary\<K, V\>
- IDictionary: Instantiates Dictionary\<string, object\>
- Custom classes or structs: Must have a default constructor without arguments.

The type `object` is also supported and the deserialization for this target
is performed according to JSON node at hand with the following mapping:

- JSON string: string
- JSON number: double
- JSON bool: bool
- JSON array: object[]
- JSON dictionary: Dictionary\<string, object\>

You may, however, want to restrict the supported types and/or support custom
types too. You can use the `RestrictTypeAttribute` for that. This attribute takes
`ObjectTypes` enum value and an optional custom types array. The `ObjectTypes`
enum define which types to look for, the default value is `ObjectTypes.JSON`
and supports all types except custom. If the custom types are supported (by
either using `ObjectTypes.All` or `ObjectTypes.Custom`) then an additional 
array of custom types can be given to try deserializing the object into them.
Custom types are classes or structs cannot be enumerable and nullable. The order
of the custom types are important as they are tried one by one. If no type can
be deserialized into, then a generic dictionary is created (unless the dictionary
type is not supported, in which case an exception is thrown).

```cs
class A
{
    public int intField;
}

class B
{
    public float floatField;
}

class C
{
    [RestrictType(ObjectTypes.Custom, customTypes = new Type[] {A, B})]
    public object field;
}

var c = JSON.Deserialize<C>("{\"field\":{\"floatField\":5}}");
c.field // object of type B with floatField 5 
```

`RestrictTypeAttribute` can also be used with IList, IList\<object\>, List\<object\>,
object[], IDictionary, IDictionary\<K,object\>, Dictionary\<K,object\>. 

### Deserialization of Extra Nodes

Sometimes you may receive more key / value pairs than what your class and struct supports.
By default, the deserializer will throw an exception in this case unless 
`ObjectOptions.IgnoreUnknownKey` is used. You can, however, also decide to parse these
extras into your class / struct. You can do that by adding a field or property with
`JSONExtrasAttribute`. The type of the field or the object must be Dictionary\<string,object\>.
The attribute can also have optional `NodeOptions`. A field or a property with this
attribute is never serialized or deserialized even if it is public.

```cs
class A
{
    [JSONExtras]
    public Dictionary<string, object> extras;
}

A a = JSON.Deserialize<A>("{\"key\":5}");
a.extras["key"] // 5
```

You can also use `RestrictTypeAttribute` to restrict the supported types or use custom
types. The extras are also used for the serialization and are serialized on the same level
as the object.

### Inheritance

You may want to provide deserialization to interface or abstract class targets. One option
would be to use a custom instantiater 
(see [Custom Deserialization with Instantiater](#custom-deserialization-with-instantiater)), however
in most cases you can also simply do that by using `ConditionalInstantiation` and 
`DefaultInstantiation` attributes. These can redirect the instantiated types for an
interface or a class. `ConditionalInstantiationAttribute` checks for a key value pair in
the received JSON node and instantiates the referenced type if there is match. If no
condition is met, then the referenced type from `DefaultInstantiationAttribute` is
instantiated. If no such attribute exists, then the default framework deserialization
is performed.

```cs
[ConditionalInstantiation(typeof(A), "type", 0)]
[ConditionalInstantiation(typeof(B), "type", 1)]
[DefaultInstantiation(typeof(C))]
interface I
{
}

class A : I
{
    public int type;
}

class B : I
{
    public int type;
}

[JSONObject(ObjectOptions.IgnoreUnknownKey)]
class C : I
{
}

I obj = JSON.Deserialize<I>("{\"type\":1}"); // obj is of type B.
```

### Constructors

You can deserialize objects with custom constructors using the `JSONConstructor`
attribute. The constructor arguments can have additional `JSONNode` attributes. Be
aware that only one constructor can have this attribute.

```cs
class AClass<T>
{
    private T _arg;

    [JSONConstructor]
    public AClass([JSONNode(key = "field")] T argument)
    {
        _arg = argument; 
    }
}
var obj = Deserialize<AClass<int>>("{\"field\":1}");
```

### Deserialization Lifecycle

You can listen to the deserialization lifecycle of an object by implementing the
`IDeserializationListener` interface.

```cs
public class AClass : IDeserializationListener
{
    void IDeserializationListener.OnDeserializationWillBegin(Deserializer deserializer)
    {
        Debug.Log ("Deserialization started.");
    }

    void IDeserializationListener.OnDeserializationSucceeded(Deserializer deserializer)
    {
        Debug.Log ("Deserialization ended successfully.");
    }

    void IDeserializationListener.OnDeserializationFailed(Deserializer deserializer)
    {
        Debug.Log ("Deserialization failed.");
    }
}
```

The `OnDeserializationWillBegin` call is always followed by either the success or
fail call. The fail method is called just before throwing an exception.

## Custom Deserialization with Instantiater

Intantiater is the component that is responsible for instantiating instances of
an object. The basic instantiater can be accessed with `Instantiater.Simple`. Every
deserializer has an instantiater associated with it. You can subclass the 
instantiater to perform your application-specific logic in 
`Instantiater.TryInstantiate`.

```cs
public class SpecialInstantiater : Instantiater
{
    protected override bool TryInstantiate (
        JSONNode node,
        Type targetType,
        Type referingType,
        NodeOptions options,
        Deserializer deserializer,
        out InstantiationData instantiationData)
    {
        if (type == typeof(MySpecialClass)) {
            // Custom deserializers can be used to instantiate classes
            // with constructors.
            instantiationData = new InstantiationData();
            instantiationData.instantiatedObject 
                = new MySpecialClass(node["key"]);
            instantiationData.needsDeserialization = true;
            instantiationData.ignoredKeys = new HashSet<string> { "key "};
            return true;
        } else {
            // Returning false will simply run the regular
            // instantiation process.
            instantiationData = InstantiationData.Null;
            return false;
        }
    }
}

Deserializer.Default.instantiater = new SpecialInstantiater();
var obj = JSON.Deserialize<MySpecialClass>(jsonString);
```

## Custom Deserialization with Deserializer

Deserializer is the actual component that performs the deserialization. The
basic deserializer can be accessed with `Deserializer.Simple`. When no specific
deserializer is given, the default deserializer is used (`Deserializer.Default`). 
The default deserializer is the simple deserializer unless set otherwise. The
deserializer has an associated `Instantiater` to instantiate instances of objects.
You can change this instantiater from `Deserializer.instantiater`. You can
create your own deserializer by simply subclassing `Deserializer`. You should then
override the `Deserializer.TryDeserializeOn` method 
to perform your application specific deserialization.

```cs
public class SpecialDeserializer : Deserializer
{
    protected override bool TryDeserialize (
        object obj,
        JSONNode node,
        NodeOptions options)
    {
        // obj is always guaranteed to be non-null.
        if (obj.GetType() == typeof(MySpecialClass)) {
            MySpecialDeserializeFunction(obj, node);
            return true;
        } else {
            // Returning false will simply run the regular
            // deserialization process.
            return false;
        }
    }
}

mySpecialObject.FeedJSON(jsonString, new SpecialDeserializer());
```

When the `Deserializer.Deserialize` method is called, it first tries to
instantiate the object with its assigned instantiater. When
the object is instantiated, the `Deserializer.DeserializeOn` method is called
on the object. This first tries the custom `Deserializer.TryDeserialize`
method, then `IDeserializable.Deserialize` if the object implements the interface,
and finally performs the regular framework deserialization if all fails.

The classes that are deserialized with the `Deserializer.TryDeserialize` method 
do not receive deserialization lifecycle calls from `IDeserializationListener`.

## Custom Deserialization with IDeserializable

Another way to provide custom deserialization is by implementing the interface
`IDeserializable`. It is important to notice that `Deserializer.TryDeserialize` is called
first and this interface will be ignored if that method returns true. In addition,
you still need to make sure that your class can be instantiated.

```cs
public class AClass : IDeserializable
{
    void IDeserializable.Deserialize(JSONNode node, Deserializer deserializer)
    {
        listField.AddRange (deserializer.DeserializeToList<int>(node["list"]));
    }
}
```

The classes that are deserialized with the `IDeserializable.Deserialize` method do 
not receive deserialization lifecycle calls from `IDeserializationListener`.

## Special Types

### Enums

Enums are by default serialized and deserialized directly with their member names. This process
can, however, be customized with the use of `JSONEnumAttribute`. The attribute allows the
following formating options:

- useIntegers: The enums are serialized / deserialized according to their numeric values.
- format: Optional formatting to be applied given in the form of `JSONEnumMemberFormating`.
It supports lowercase, uppercase or captialize (only the first letter is captialized).
- prefix: Adds an optional prefix to the formatted member name.
- suffix: Adds an optional suffix to the formatted member name.

```cs
[JSONEnum(format = JSONEnumMemberFormating.Lowercased, suffix = "Position")]
public enum Positions
{
    Forward
}

JSON.Serialize(Positions.Forward) // forwardPosition
JSON.DeserializeEnum<Positions>("forwardPosition") // Positions.Forward
```

### Tuples

Classes and structs can also be serialized and deserialized as tuples (JSON arrays). This
is performed by adding the `ObjectOptions.TupleFormat` to a `JSONObjectAttribute` at
the class / struct declaration. Tuple formatted classes and structs always ignore properties.
When serialized, the fields are serialized in an array in the order they are declared.
When deserialized, the class / struct MUST provide a constructor with the 
`JSONConstructorAttribute` where the arguments are passed directly from the array.

```cs
[JSONObject(ObjectOptions.TupleFormat)]
public class Tuple<T1, T2>
{
    [JSONNode(NodeOptions.SerializeNull)]
    public T1 item1;

    [JSONNode(NodeOptions.SerializeNull)]
    public T2 item2;

    [JSONConstructor]
    public Tuple(T1 item1, T2 item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }
}

var tuple = new Tuple<int, string>(2, "value");
tuple.ToJSONString(); // [2, "value"]

var obj = JSON.Deserialize<Tuple<IList, Tuple<float, float>>>(
    "[[\"this\",\"is\",\"IList\"], [3.14, 2.17]]");
```

Please notice that tuple deserialization takes place at the instantiation and therefore
cannot be used together with `Deserializer.DeserializeOn`.

UnityJSON does not use C# tuples because Unity3D does not have support for them yet.

## Changelog

### v2.2

- Bug fixes
- Adds generic deserialize methods to `Deserializer`

### v2.1

- Provides Tuple support

### v2.0

- Bug fixes
- Adds `Serializer.SerializeByParts`
- Adds `Deserializer.DeserializeByParts` and deserializer methods taking JSON
string arguments
- Creates the class `Instantiater`
- Allows use of `RestrictTypeAttribute` with constructor arguments
- Introduces `InstantiationData` to work around ignored keys

### v1.1

- Adds `JSONConstructorAttribute`
- Fixes conditional instantiation bug: JSONNode kept the same after key removal
