using Analyzer.Core;
using System;
using System.Collections.Generic;
using Worosoft.Xamarin.CommonTypes.Operations;

namespace Analyzer.Services
{
    public interface IDataStoreService
    {
        IEnumerable<T> LoadModels<T>();
        OperationResult<IEnumerable<T>> SaveModels<T>(IEnumerable<Tuple<T, OperationKinds>> modelOperations) where T : IModelDefinition;

        void SaveTemporarily<T>(T value) where T : class, IModelDefinition;
        void ClearTemporaryCache<T>(T value = null) where T : class, IModelDefinition;
    }
}
