namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;

	public class ParamRepTemplate : IEquatable<ParamRepTemplate>, IApplyTypeArgs
	{
		private TypeRepRef _type = null;
		public TypeRepRef Type { 
			get { return _type; }
			set {
				_type=value;
			}
		}

		public string Name { get; set; }

		// ref or out param?
		[XmlAttribute("byref")]
		[System.ComponentModel.DefaultValueAttribute(false)]
		public bool IsByRef{ get; set; }

		public ParamRepTemplate ()
		{
			IsByRef = false;
		}

		public ParamRepTemplate(ParamRepTemplate copyFrom)
		{

			if (copyFrom.Type != null)
			{
				Type = new TypeRepRef(copyFrom.Type);
			}

			if (!String.IsNullOrEmpty(copyFrom.Name))
			{
				Name = copyFrom.Name;
			}
			IsByRef = copyFrom.IsByRef;
		}

		public ParamRepTemplate (string t, string a)
		{
			Type = new TypeRepRef(t);
			Name = a;
			IsByRef = false;
		}

		public ParamRepTemplate (string t, string a, bool isbyref)
		{
			Type = new TypeRepRef(t);
			Name = a;
			IsByRef = isbyref;
		}

		public void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			Type.SubstituteInType(args);
		}

		#region Equality
		public bool Equals (ParamRepTemplate other)
		{
			if (other == null)
				return false;
			
			return Type == other.Type && Name == other.Name && IsByRef == other.IsByRef;
		}

		public override bool Equals (object obj)
		{
			
			ParamRepTemplate temp = obj as ParamRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (ParamRepTemplate a1, ParamRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (ParamRepTemplate a1, ParamRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = Type != null ? Type.GetHashCode() : 0;

			return hashCode ^ (Name ?? String.Empty).GetHashCode () ^ IsByRef.GetHashCode();
		}
		#endregion
	}
}