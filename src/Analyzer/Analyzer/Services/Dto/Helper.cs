using Analyzer.Framework;
using Analyzer.Operations;
using System;
using System.Collections.Generic;

namespace Analyzer.Services.Dto
{
    public static class Helper
    {
        public static Tuple<T, OperationKinds> TransformToDto<VM, T>(this VM model )
            where VM: ActionableBaseViewModel<VM, T>
            where T: class, Core.IModelDefinition
        {
            Core.CoreHelpers.ValidateDefined(model, "model from transfromDto");
            var viewstate = model.State;
            var updateModel = model.CurrentItem;

            switch(viewstate)
            {
                case Core.ViewStates.Creating:
                case Core.ViewStates.CreatingViewing:
                case Core.ViewStates.UpdatingCreate:
                    return new Tuple<T, OperationKinds>(updateModel, OperationKinds.Create);
                case Core.ViewStates.Deleting:
                    return new Tuple<T, OperationKinds>(updateModel, OperationKinds.Delete);
                case Core.ViewStates.Updating:
                case Core.ViewStates.UpdatingViewing:
                    return new Tuple<T, OperationKinds>(updateModel, OperationKinds.Update);
                case Core.ViewStates.Viewing:
                    if (model.InitialState == Core.ViewStates.Creating)
                    {
                        return new Tuple<T, OperationKinds>(updateModel, OperationKinds.Create);
                    }
                    else if (model.InitialState == Core.ViewStates.Updating)
                    {
                        return new Tuple<T, OperationKinds>(updateModel, OperationKinds.Update);
                    }
                    break;
                default:
                    break;
            }

            var translateExtension = new Analyzer.Extensions.TranslateExtension();
            translateExtension.Text = "TransformToDtoInvalidViewModelViewState";
            var errorMessage = translateExtension.ProvideValue(null) as string;
            throw new InvalidOperationException(errorMessage);

        }
        public static IEnumerable<Tuple<T, OperationKinds>> TransformToDtos<VM, T>(this IEnumerable<VM> models)
            where VM : ActionableBaseViewModel<VM, T>
            where T : class, Core.IModelDefinition
        {
            Core.CoreHelpers.ValidateDefined(models, "models from transfromDto");

            var transformedDtos = new List<Tuple<T, OperationKinds>>();

            foreach(var model in models)
            {
                var transformedDto = model.TransformToDto<VM, T>();
                transformedDtos.Add(transformedDto);
            }

            return transformedDtos;
        }
    }
}
