namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;

	public class CastRepTemplate : TranslationBase, IEquatable<CastRepTemplate>
	{
		// From and To are fully qualified types
		private TypeRepRef _from;
		public TypeRepRef From { 
			get { return _from; }
			set {
				_from = value; 
			}
		}		

		private TypeRepRef _to;
		public TypeRepRef To { 
			get { return _to; }
			set {
				_to= value;
			}
		}


		public CastRepTemplate()
			: base()
		{
		}

		public CastRepTemplate(TypeRepTemplate parent, CastRepTemplate copyFrom)
			: base(parent, copyFrom)
		{
			if (copyFrom.From != null)
			{
				From = new TypeRepRef(copyFrom.From);
			}
			if (copyFrom.To != null)
			{
				To = new TypeRepRef(copyFrom.To);
			}

		}

		public CastRepTemplate (string fType, string tType, string[] imps, string java) : base(imps, java)
		{
			From = new TypeRepRef(fType);
			To = new TypeRepRef(tType);
		}

		public CastRepTemplate (string fType, string tType) : this(fType, tType, null, null)
		{
		}
		
		public override string[] mkImports() {
			if (SurroundingType != null) {
				return new string[] {SurroundingType.TypeName};
			}
			else {
				return null;
			}
		}	
		
		public override string mkJava() {
			if (From == null || To == null) {
				return null;
			}
			else {
				if (SurroundingType != null) {
					String myType = SurroundingType.TypeName.Substring(SurroundingType.TypeName.LastIndexOf('.') + 1);
					String toType = To.Type.Substring(To.Type.LastIndexOf('.') + 1);
					if (myType == toType)
					{
						// just overload various casts to my type
						return  myType + ".__cast(${expr})";
					}
					else
					{
						return  myType + ".__cast${TYPEOF_totype}(${expr})";                                        
					}
				}
				else
				{
					return "__cast_" + To.Type.Replace('.','_') + "(${expr})";
				}
			}
		}

		public override void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			if (From != null)
			{
				From.SubstituteInType(args);
			}
			if (To != null)
			{ 
				To.SubstituteInType(args);
			}
			base.Apply(args);
		}

		#region Equality
		public bool Equals (CastRepTemplate other)
		{
			if (other == null)
				return false;
			
			return From == other.From && To == other.To && base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			CastRepTemplate temp = obj as CastRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (CastRepTemplate a1, CastRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (CastRepTemplate a1, CastRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = From != null ? From.GetHashCode() : 0;
			hashCode = hashCode ^ (To != null ? To.GetHashCode() : 0);


			return hashCode ^ base.GetHashCode();
		}
		#endregion
		
	}
}