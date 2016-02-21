namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;

	[XmlType("UnknownType")]
	// For now, making as compatible as we can so inheriting from struct
	public class UnknownRepTemplate : StructRepTemplate, IEquatable<UnknownRepTemplate>
	{

		public UnknownRepTemplate ()
		{
		}

		public UnknownRepTemplate (string typeName) : base(typeName)
		{
			// If we are creating an UnknownRepTemplate for System.Object then don't
			// inherit from ourselves, else we get stack overflow when resolving.
			// This should only happen in the case that we don't have a valid
			// net-templates-dir with a definition for System.Object.
			if (typeName != "System.Object") {
				Inherits = new String[] { "System.Object" };
			} else {
				Inherits = new String[] { };
			}
		}

		public UnknownRepTemplate (TypeRepRef typeName) : this(typeName.Type)
		{
		}

		public UnknownRepTemplate (UnknownRepTemplate copyFrom) : base(copyFrom)
		{
		}

		public override string[] Imports { 
			get {
				return new string[0];
			}
		}

		public override string mkJava() {
			return TypeName;
		}

		public override TypeRepTemplate Instantiate(ICollection<TypeRepTemplate> args)
		{
			UnknownRepTemplate copy = new UnknownRepTemplate(this);
			// don't instantiate, its an unknown type: copy.Apply(mkTypeMap(args));
			return copy;
		}
		
		#region Equality
		public bool Equals (UnknownRepTemplate other)
		{
			return base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			UnknownRepTemplate temp = obj as UnknownRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (UnknownRepTemplate a1, UnknownRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (UnknownRepTemplate a1, UnknownRepTemplate a2)
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