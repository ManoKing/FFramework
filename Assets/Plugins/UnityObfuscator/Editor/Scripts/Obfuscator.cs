﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil;
using System.Linq;
using UnityEngine.EventSystems;

namespace Flower.UnityObfuscator
{
    internal abstract class Obfuscator
    {
        //为true时只混特定范围，即ObfuscateList下的配置
        //为false时，除了白名单外都混，即WhiteList下的配置
        //protected bool OnlyParticularRange = false;

        protected ObfuscateType obfuscateType;

        protected WhiteList whiteList;
        protected WhiteList obfuscateList;

        protected abstract string Symbol { get; }

        public abstract void Init(ObfuscateType obfuscateType);

        public abstract bool MethodNeedSkip(MethodDefinition method);

        public abstract bool ClassNeedSkip(TypeDefinition type);

        protected bool IsChange(bool inObfuscateList, bool inWhiteList)
        {
            bool change = true;

            switch (obfuscateType)
            {
                case ObfuscateType.ParticularRange:
                    change = inObfuscateList;
                    break;
                case ObfuscateType.WhiteList:
                    change = !inWhiteList;
                    break;
                case ObfuscateType.Both:
                    change = inObfuscateList && !inWhiteList;
                    break;
                default:
                    change = false;
                    break;
            }

            return change;
        }

        protected virtual bool IsChangeField(TypeDefinition t, FieldDefinition f)
        {
            if ((f.Attributes & FieldAttributes.Public) != 0)
                return false;
            if (f.CustomAttributes.Any(x => x.AttributeType.FullName == "UnityEngine.SerializeField"))
                return false;
            var fieldName = f.FullName;
            bool inObfuscateList = ((obfuscateList.IsWhiteListNamespace(t.Namespace)
                || obfuscateList.IsWhiteListClassMember(fieldName, t.Name, t.Namespace)
                || obfuscateList.IsWhiteListClass(t.Name, t.Namespace))
                || obfuscateList.IsWhiteListClassNameOnly(t.Name, t.Namespace)
                && !ClassNeedSkip(t));

            bool inWhiteList = (whiteList.IsWhiteListNamespace(t.Namespace)
                || whiteList.IsWhiteListClassMember(fieldName, t.Name, t.Namespace)
                || whiteList.IsWhiteListClass(t.Name, t.Namespace)
                || ClassNeedSkip(t));

            return IsChange(inObfuscateList, inWhiteList);
        }

        protected virtual bool IsChangeProperty(TypeDefinition t, PropertyDefinition p)
        {
            if (p.GetMethod != null && (p.GetMethod.Attributes & MethodAttributes.Public) != 0)
                return false;
            if (p.SetMethod != null && (p.SetMethod.Attributes & MethodAttributes.Public) != 0)
                return false;
            var propertyName = p.FullName;
            bool inObfuscateList = (obfuscateList.IsWhiteListNamespace(t.Namespace)
                || obfuscateList.IsWhiteListClassMember(propertyName, t.Name, t.Namespace)
                || obfuscateList.IsWhiteListClass(t.Name, t.Namespace))
                || obfuscateList.IsWhiteListClassNameOnly(t.Name, t.Namespace)
                && !ClassNeedSkip(t);

            bool inWhiteList = (whiteList.IsWhiteListNamespace(t.Namespace)
                || whiteList.IsWhiteListClassMember(propertyName, t.Name, t.Namespace)
                || whiteList.IsWhiteListClass(t.Name, t.Namespace)
                || ClassNeedSkip(t));

            return IsChange(inObfuscateList, inWhiteList);
        }

        protected virtual bool IsChangeMethod(TypeDefinition t, MethodDefinition method)
        {
            if ((method.Attributes & MethodAttributes.Public) != 0)
                return false;
            bool inObfuscateList = (obfuscateList.IsWhiteListNamespace(t.Namespace)
                || obfuscateList.IsWhiteListMethod(method.Name, t.Name, t.Namespace)
                || obfuscateList.IsWhiteListClass(t.Name, t.Namespace)
                || obfuscateList.IsWhiteListClassNameOnly(t.Name, t.Namespace))
                && !ClassNeedSkip(t) && !MethodNeedSkip(method);

            bool inWhiteList = (MethodNeedSkip(method)
                || whiteList.IsWhiteListNamespace(t.Namespace)
                || whiteList.IsWhiteListMethod(method.Name, t.Name, t.Namespace)
                || whiteList.IsWhiteListClass(t.Name, t.Namespace)
                || ClassNeedSkip(t));

            return IsChange(inObfuscateList, inWhiteList);
        }
        private bool IsUnityComponent(TypeDefinition t)
        {
            if (t.BaseType == null)
                return false;
            var baseType = t.BaseType;
            if (baseType.FullName == typeof(Component).FullName)
                return true;
            if (baseType.FullName == typeof(MonoBehaviour).FullName)
                return true;
            if (baseType.FullName == typeof(UIBehaviour).FullName)
                return true;
            var baseTypeDef = baseType.Resolve();
            if (baseTypeDef != null)
                return IsUnityComponent(baseTypeDef);
            return false;
        }
        protected virtual bool IsChangeClass(TypeDefinition t)
        {
            if (IsUnityComponent(t))
                return false;
            // if (t.Interfaces.Any(x => x.InterfaceType.FullName == typeof(ILuaWrap).FullName))
            //     return true;
            if ((t.Attributes & TypeAttributes.Public) != 0)
                return false;
            bool inObfuscateList = ((obfuscateList.IsWhiteListNamespace(t.Namespace)
                || obfuscateList.IsWhiteListClass(t.Name, t.Namespace)
                && !obfuscateList.IsWhiteListClassNameOnly(t.Name, t.Namespace))
                && !ClassNeedSkip(t));

            bool inWhiteList = (whiteList.IsWhiteListNamespace(t.Namespace)
                || whiteList.IsWhiteListClass(t.Name, t.Namespace)
                || whiteList.IsWhiteListClassNameOnly(t.Name, t.Namespace)
                || ClassNeedSkip(t));

            return IsChange(inObfuscateList, inWhiteList);
        }

        protected virtual bool IsChangeNamespace(TypeDefinition t)
        {
            bool inObfuscateList = obfuscateList.IsWhiteListNamespace(t.Namespace) && !obfuscateList.IsWhiteListNamespcaeNameOnly(t.Namespace);
            bool inWhiteList = whiteList.IsWhiteListNamespace(t.Namespace, true) || whiteList.IsWhiteListNamespcaeNameOnly(t.Namespace, true);

            return IsChange(inObfuscateList, inWhiteList);
        }

    }
}


