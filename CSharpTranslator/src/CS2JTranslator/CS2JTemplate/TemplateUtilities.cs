/*
   Copyright 2010-2013 Kevin Glynn (kevin.glynn@twigletsoftware.com)
   Copyright 2007-2013 Rustici Software, LLC

This program is free software: you can redistribute it and/or modify
it under the terms of the MIT/X Window System License

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

You should have received a copy of the MIT/X Window System License
along with this program.  If not, see 

   <http://www.opensource.org/licenses/mit-license>
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

// These Template classes are in-memory versions of the xml translation templates
// (we use C# to directly persist to / from files).

// We have overloaded Equals to test value equality for these template objects.  For now its only 
// used by unit tests (to check that the object survives xml serialization / deserialization
// unscathed). But it might be useful down the road.
// By overloading Equals, we also have to overload GetHashCode (well, its highly reccomended)...

namespace Twiglet.CS2J.Translator.TypeRep
{
   
   public class TemplateUtilities
   {
       public static readonly bool DO_IMPLICIT_CASTS=false;
      public static string Substitute(string c, Dictionary<string,TypeRepTemplate> argMap)
      {
         String ret = c;
         if (argMap.ContainsKey(c))
         {
            ret = argMap[c].TypeName;
         }
         return ret;
      }

      private class TypeVarMapper
      {
         private Dictionary<string,TypeRepTemplate> myArgMap;

         public TypeVarMapper(Dictionary<string,TypeRepTemplate> inArgMap)
         {
            myArgMap = inArgMap;
         }

         public string ReplaceFromMap(Match m)
         {
            if (myArgMap.ContainsKey(m.Value))
            {
               return myArgMap[m.Value].mkSafeTypeName();
            }
            return m.Value;
         }
      }

      public static string SubstituteInType(String type, Dictionary<string,TypeRepTemplate> argMap)
      {
         if (String.IsNullOrEmpty(type) || argMap == null)
            return type;

         TypeVarMapper mapper = new TypeVarMapper(argMap);
         return Regex.Replace(type, @"([\w|\.]+)*", new MatchEvaluator(mapper.ReplaceFromMap));
      }

      
      private static string OldSubstituteInType(String type, Dictionary<string,TypeRepTemplate> argMap)
      {
         if (String.IsNullOrEmpty(type))
            return type;

         string ret = type;
         // type is either "string" or "string*[type,type,...]*" or string[]
//         Match match = Regex.Match(type, @"^([\w|\.]+)(?:\s*\*\[\s*([\w|\.]+)(?:\s*,\s*([\w|\.]+))*\s*\]\*)?$");
         Match match = Regex.Match(type, @"([\w|\.]+)*");
         if (match.Success)
         {
            CaptureCollection captures = match.Captures;
            StringBuilder buf = new StringBuilder();
            buf.Append(Substitute(captures[0].Value, argMap));
            if ( captures.Count > 1)
            {
               bool first = true;
               buf.Append("[");
               for (int i = 1; i < captures.Count; i++)
               {
                  if (!first)
                  {
                     buf.Append(", ");
                  }
                  buf.Append(Substitute(captures[i].Value, argMap));
                  first = false;
               }
               buf.Append("]");
            }
            ret = buf.ToString();
         }
         return ret;
      }
   }

	// Simple <type> <name> pairs to represent formal parameters

	// Represents a variable number of parameters

	// A namespace alias entry.


	// Never directly create a TranslationBase. Its a common root for translatable language entities

	// Method has the same info as a delegate as a constructor plus a name and return type

	//  A user-defined cast from one type to another

	// A member field definition

	// A property definition.  We need separate java translations for getter and setter.  If JavaGet is null
   // then we can use Java (from TranslationBase) as the translation for gets.
   // Imports are shared between getter and setter (might lead to some unneccessary extra imports, but I'm
   // guessing that normally imports will be the same for both)

	// An indexer is like a unnamed property that has params

	// A member of an enum,  may also have a numeric value

	// Base Template for classes, interfaces, enums, etc.

	// Base Template for classes, interfaces, etc.


	// Used when the result of resolving the callee of an APPLY node is a pointer to a delegate
}
