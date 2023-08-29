using System;
using System.Reflection;
using System.Collections;

namespace Sinan.AMF3
{
    /// <summary>
    /// This type supports the Fluorine infrastructure and is not intended to be used directly from your code.
    /// </summary>
    public sealed class ClassDefinition
    {
        private string m_className;
        private ClassMember[] m_members;
        private bool m_externalizable;
        private bool m_dynamic;

        internal static ClassMember[] EmptyClassMembers = new ClassMember[0];

        internal ClassDefinition(string className, ClassMember[] members, bool externalizable, bool isDynamic)
        {
            m_className = className;
            m_members = members;
            m_externalizable = externalizable;
            m_dynamic = isDynamic;
        }

        /// <summary>
        /// Gets the class name.
        /// </summary>
        public string ClassName { get { return m_className; } }
        /// <summary>
        /// Gets the class member count.
        /// </summary>
        public int MemberCount
        {
            get
            {
                if (m_members == null)
                    return 0;
                return m_members.Length;
            }
        }
        /// <summary>
        /// Gets the array of class members.
        /// </summary>
        public ClassMember[] Members { get { return m_members; } }
        /// <summary>
        /// Indicates whether the class is externalizable.
        /// </summary>
        public bool IsExternalizable { get { return m_externalizable; } }
        /// <summary>
        /// Indicates whether the class is dynamic.
        /// </summary>
        public bool IsDynamic { get { return m_dynamic; } }
        /// <summary>
        /// Indicates whether the class is typed (not anonymous).
        /// </summary>
        public bool IsTypedObject { get { return (m_className != null && m_className != string.Empty); } }
    }

    /// <summary>
    /// This type supports the Fluorine infrastructure and is not intended to be used directly from your code.
    /// </summary>
    public sealed class ClassMember
    {
        string _name;
        BindingFlags _bindingFlags;
        MemberTypes _memberType;

        internal ClassMember(string name, BindingFlags bindingFlags, MemberTypes memberType)
        {
            _name = name;
            _bindingFlags = bindingFlags;
            _memberType = memberType;
        }
        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// Gets the member binding flags.
        /// </summary>
        public BindingFlags BindingFlags
        {
            get { return _bindingFlags; }
        }
        /// <summary>
        /// Gets the member type.
        /// </summary>
        public MemberTypes MemberType
        {
            get { return _memberType; }
        }
    }
}
