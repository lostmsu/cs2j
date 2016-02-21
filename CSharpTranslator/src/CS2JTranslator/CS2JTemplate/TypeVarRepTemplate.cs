namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using Twiglet.CS2J.Translator.Utils;

	[XmlType("TypeVariable")]
	// Represents Type Variables.  We inherit from ClassRepTemplate to that
	// Type Variables have the same interface as types, but we can override as
	// neccessary
	public class TypeVarRepTemplate : ClassRepTemplate, IEquatable<TypeVarRepTemplate>
	{

		public TypeVarRepTemplate ()
		{
		}

		public TypeVarRepTemplate (string typeName) : base(typeName)
		{
			Inherits = new String[] { "System.Object" };
		}

		public TypeVarRepTemplate (TypeVarRepTemplate copyFrom) : base(copyFrom)
		{
		}

		public override string[] Imports { 
			get {
				return new string[0];
			}
		}

		public override TypeRepTemplate Instantiate(ICollection<TypeRepTemplate> args)
		{
			TypeVarRepTemplate copy = new TypeVarRepTemplate(this);
			if (args != null && args.Count > 0)
			{
				copy.TypeName = args.GetEnumerator().Current.TypeName;
			}
			return copy;
		}
		
		public override bool IsA (TypeRepTemplate other,  DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast) {
			return base.IsA(other, AppEnv,implicitCast);                         
		}

		#region Equality
		public bool Equals (TypeVarRepTemplate other)
		{
			return base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			TypeVarRepTemplate temp = obj as TypeVarRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (TypeVarRepTemplate a1, TypeVarRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (TypeVarRepTemplate a1, TypeVarRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
		#endregion
		
		
	}
}