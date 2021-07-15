namespace Worosoft.Xamarin.CommonTypes.Operations
{
    public interface IOperationsService
    {
        //OperationResult<IEnumerable<xxxConcreteType>> UpdatexxxConcreteType(IEnumerable<ActionableOperations<xxxConcreteType>> operations);
    }

    public enum OperationKinds
    {
        Update,
        Create,
        Delete,
        Increase,
        Decrease
    }

    public class ActionableOperations<T>
    {
        public ActionableOperations(T item, OperationKinds operationKind)
        {
            this.Item = item;
            this.OperationKind = operationKind;
        }

        public T Item { get; private set; }
        public OperationKinds OperationKind { get; private set; }
    }
}
