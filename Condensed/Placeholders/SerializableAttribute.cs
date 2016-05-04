using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{

    /// <summary>
    /// Dummy SerializableAttribute to support the Portable Class Library build of this library.
    /// </summary>
    /// <exclude />
    [System.AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate,
        Inherited = false, AllowMultiple = false
        )]
    sealed class SerializableAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236

        public SerializableAttribute()
        {
        }

    }
    
}
