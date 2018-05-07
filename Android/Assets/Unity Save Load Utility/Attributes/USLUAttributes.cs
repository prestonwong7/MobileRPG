using System;

//These are the attributes that are used by the Unity Save load Utility.


//The main class, which is not used as an attribute itself
public class USLUAttribute : Attribute {

}

//If a class does NOT have the SaveNoMembers attribute, then all field that have the DontSaveMember attribute won't be saved.
public class DontSaveMember : USLUAttribute {
	
}

//If a class DOES have the SaveNoMembers attribute, then only those fields that have the SaveMember attribute will be saved.
public class SaveMember : USLUAttribute {

}

//Use this attribute for the class (not field) if the majority of fields should not be saved.
public class SaveNoMembers : USLUAttribute {

}
