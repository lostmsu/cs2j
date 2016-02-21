namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Xml.Serialization;

	public class PropRepTemplate : FieldRepTemplate, IEquatable<PropRepTemplate>
	{
		protected string _javaGet = null;		
		[XmlElement("Get")]
		public virtual string JavaGet {
			get {
				if (!CanRead) return null;
				if (_javaGet == null) {
					if (_java == null) {
						return (CanRead ? "${this:16}.get" + Name + "()" : null);
					}
					else {
						return _java;
					}
				}
				else {
					return _javaGet;
				}
			}
			set { _javaGet = value; }
		}
                
/*
      public override string Java
      {
         get
         {
            return JavaGet;
         }
      }
      */

		protected string _javaSet = null;
		[XmlElement("Set")]
		public virtual string JavaSet { 
			get {
				if (_javaSet == null) {
					return (CanWrite ? "${this:16}.set" + Name + "(${value})" : null);
				}
				else {
					return _javaSet;
				}
			}
			set { _javaSet = value; }
		}
		
		// canRead?
		private bool _canRead = true;
		[XmlAttribute("read")]
		[System.ComponentModel.DefaultValueAttribute(true)]
		public bool CanRead { 
			get { return _canRead; }
			set { _canRead = value; }
		}
		
		// canWrite?
		private bool _canWrite = true;
		[XmlAttribute("write")]
		[System.ComponentModel.DefaultValueAttribute(true)]
		public bool CanWrite { 
			get { return _canWrite; }
			set { _canWrite = value; }
		}

		public PropRepTemplate()
			: base()
		{
		}

		public PropRepTemplate(TypeRepTemplate parent, PropRepTemplate copyFrom)
			: base(parent, copyFrom)
		{
			if (!String.IsNullOrEmpty(copyFrom.JavaGet))
			{
				JavaGet = copyFrom.JavaGet;
			}
			if (!String.IsNullOrEmpty(copyFrom.JavaSet))
			{
				JavaSet = copyFrom.JavaSet;
			}
			CanRead = copyFrom.CanRead;
			CanWrite = copyFrom.CanWrite;
		}

		public PropRepTemplate(string fType, string fName, string[] imps, string javaGet, string javaSet)
			: base(fType, fName, imps, null)
		{
			JavaGet = javaGet;
			JavaSet = javaSet;
		}

		public PropRepTemplate (string fType, string fName) : this(fType, fName, null, null, null)
		{
		}
		
		public override string mkJava ()
		{
			// favour JavaGet
			return JavaGet;
		}
		
		#region Equality
		public bool Equals (PropRepTemplate other)
		{
			if (other == null)
				return false;
			
			return JavaGet == other.JavaGet && JavaSet == other.JavaSet && base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			PropRepTemplate temp = obj as PropRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (PropRepTemplate a1, PropRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (PropRepTemplate a1, PropRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			return (JavaGet ?? String.Empty).GetHashCode () ^ (JavaSet ?? String.Empty).GetHashCode () ^ base.GetHashCode ();
		}
		#endregion		
	}
}