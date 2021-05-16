using Analyzer.Core;
using Analyzer.Operations;
using System;
using System.Collections.Generic;

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
