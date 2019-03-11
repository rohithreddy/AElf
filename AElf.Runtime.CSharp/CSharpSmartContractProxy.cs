using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using AElf.Kernel;
using AElf.Kernel.SmartContract;
using AElf.Kernel.SmartContract.Contexts;
using AElf.Kernel.SmartContract.Sdk;

namespace AElf.Runtime.CSharp
{
    public class CSharpSmartContractProxy
    {
        private static MethodInfo GetMethedInfo(Type type, string name)
        {
            return type.GetMethod(name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        }

        private object _instance;

        private Dictionary<string, MethodInfo> _methodInfos = new Dictionary<string, MethodInfo>();

        public CSharpSmartContractProxy(object instance)
        {
            _instance = instance;
            InitializeMethodInfos(_instance.GetType());
        }

        private void InitializeMethodInfos(Type instanceType)
        {
            _methodInfos = new[]
            {
                nameof(GetChanges),nameof(SetStateProvider),nameof(Cleanup),nameof(InternalInitialize)
            }.ToDictionary(x => x, x => GetMethedInfo(instanceType, x));
        }


        public void SetStateProvider(IStateProvider stateProvider)
        {
            _methodInfos[nameof(SetStateProvider)].Invoke(_instance, new object[] {stateProvider});
        }

        public void InternalInitialize(ISmartContractBridgeContext context)
        {
            _methodInfos[nameof(InternalInitialize)].Invoke(_instance, new object[] {context});
        }

        public TransactionExecutingStateSet GetChanges()
        {
            return (TransactionExecutingStateSet) _methodInfos[nameof(GetChanges)]
                .Invoke(_instance, new object[0]);
        }

        internal void Cleanup()
        {
            _methodInfos[nameof(Cleanup)].Invoke(_instance, new object[0]);
        }
    }
}