namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Xml.Serialization;

	public class TypeRepRef
	{

		private class TypeVarMapper
		{
			private Dictionary<string,TypeRepTemplate> myArgMap;
			private TypeRepRef inTy;

			public TypeVarMapper(Dictionary<string,TypeRepTemplate> inArgMap, TypeRepRef ty)
			{
				myArgMap = inArgMap;
				inTy = ty;
			}

			public string ReplaceFromMap(Match m)
			{
				if (myArgMap.ContainsKey(m.Value))
				{
					// If the replacement type is primitive then tell cs2j to use the Boxed version when emitting type. 
					inTy.ForceBoxed = true;
					return myArgMap[m.Value].mkSafeTypeName();
				}
				return m.Value;
			}
		}

		// if ForceBoxed is true then any primitive types should be emitted as the boxed rep.
		private bool _forceBoxed = false;
		[XmlAttribute("box")]
		[System.ComponentModel.DefaultValueAttribute(false)]
		public bool ForceBoxed
		{
			get
			{
				return _forceBoxed;
			}
			set
			{
				_forceBoxed = value;
			}
		}

		private string _type = "";
		[XmlText]
		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value.Replace("<","*[").Replace(">","]*");
			}
		}

		public TypeRepRef()
		{
         
		}

		public TypeRepRef(TypeRepRef copyFrom)
		{
			ForceBoxed = copyFrom.ForceBoxed;
			Type = copyFrom.Type;
		}

		public TypeRepRef(string t)
		{
			ForceBoxed = false;
			Type = t;
		}

		public void SubstituteInType(Dictionary<string,TypeRepTemplate> argMap)
		{
			if (!String.IsNullOrEmpty(Type))
			{
				TypeVarMapper mapper = new TypeVarMapper(argMap, this);
				Type = Regex.Replace(Type, @"([\w|\.]+)*", new MatchEvaluator(mapper.ReplaceFromMap));
			}
		}

		// public static implicit operator string(TypeRepRef t) 
		// {
		//    return t.ToString();
		// }

		public override string ToString()
		{
			return Type;
		}

		#region Equality
		public bool Equals (TypeRepRef other)
		{
			if (other == null)
				return false;

			return ForceBoxed == other.ForceBoxed && Type == other.Type;
		}

		public override bool Equals (object obj)
		{
			
			TypeRepRef temp = obj as TypeRepRef;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (TypeRepRef a1, TypeRepRef a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (TypeRepRef a1, TypeRepRef a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			return (Type ?? String.Empty).GetHashCode () ^ ForceBoxed.GetHashCode();
		}
		#endregion
	}
}