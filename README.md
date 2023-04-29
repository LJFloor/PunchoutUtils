# PunchoutUtils
A nifty utility pack for SAP OCI Punch-outs for dotnet

## Features
For now there is only one feature
- Serialize and deserialize a punch-out

## Constants
In this document, we will use the following string as a punch-out:
```
NEW_ITEM-DESCRIPTION%5B0%5D%3DRubber%20duck&NEW_ITEM-QUANTITY%5B0%5D%3D1&NEW_ITEM-DESCRIPTION%5B1%5D%3DData%20structures%20in%20PHP%20%28book%29&NEW_ITEM-QUANTITY%5B1%5D%3D1
```
Below is a neatly formatted version so you can read it:
```
NEW_ITEM-DESCRIPTION[0]=Rubber duck&
NEW_ITEM-QUANTITY[0]=1&
NEW_ITEM-DESCRIPTION[1]=Data structures in PHP (book)&
NEW_ITEM-QUANTITY[1]=1&
```

Of course, there are many more fields as documented: [https://punchoutcommerce.com/docs/sap-oci-5.pdf](https://punchoutcommerce.com/docs/sap-oci-5.pdf#%5B%7B%22num%22%3A50%2C%22gen%22%3A0%7D%2C%7B%22name%22%3A%22XYZ%22%7D%2C112.5%2C726.25%2C0%5D)

## Serialize / Deserialize
```csharp
// Deserialize
var entries = PunchoutSerializer.Deserialize("NEW_ITEM-DESCRIPTION%5B0%5D%3DRubber%20duck&NEW_ITEM-QUANTITY%5B0%5D%3D1&NEW_ITEM-DESCRIPTION%5B1%5D%3DData%20structures%20in%20PHP%20%28book%29&NEW_ITEM-QUANTITY%5B1%5D%3D1");

// Serialize back to punch-out
var text = PunchoutSerializer.Serialize(entries);		

// Or a single item 
PunchoutSerialize.Serialize(entries.First());
```

You can also deserialize to your custom model you use for your webshop or your ERP system. Make sure you map the correct fields to your model:
```csharp
public class MyCustomModel 
{
	[FieldName("DESCRIPTION[n]")]
	public string Description { get; set; }

	[FieldName("QUANTITY[n]")]
	public int Quantity { get; set; }
}
```

A list of valid field names can be found here: [https://punchoutcommerce.com/docs/sap-oci-5.pdf](https://punchoutcommerce.com/docs/sap-oci-5.pdf#%5B%7B%22num%22%3A50%2C%22gen%22%3A0%7D%2C%7B%22name%22%3A%22XYZ%22%7D%2C112.5%2C726.25%2C0%5D).
The `n` will be replaced by the index of the item.

```csharp
var entries = PunchoutSerializer.Deserialize<MyCustomModel>("NEW_ITEM-DESCRIPTION%5B0%5D%3DRubber%20duck&NEW_ITEM-QUANTITY%5B0%5D%3D1&NEW_ITEM-DESCRIPTION%5B1%5D%3DData%20structures%20in%20PHP%20%28book%29&NEW_ITEM-QUANTITY%5B1%5D%3D1");
```

## Bugs
Please create an issue if you find anything that doesn't seem right.
