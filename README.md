# PunchoutUtils
A nifty utility pack for SAP OCI Punch-outs

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
var punchout = PunchoutSerializer.Deserialize("NEW_ITEM-DESCRIPTION%5B0%5D%3DRubber%20duck&NEW_ITEM-QUANTITY%5B0%5D%3D1&NEW_ITEM-DESCRIPTION%5B1%5D%3DData%20structures%20in%20PHP%20%28book%29&NEW_ITEM-QUANTITY%5B1%5D%3D1");

// Serialize back to punch-out
var text = PunchoutSerializer.Serialize(punchout);

// Or serialize to JSON
var json = JsonSerializer.Serialize(punchout);
```

## Bugs
Please create an issue if you find anything that doesn't seem right.
