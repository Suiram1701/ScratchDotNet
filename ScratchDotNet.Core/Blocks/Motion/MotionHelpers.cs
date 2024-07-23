using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Services.Interfaces;
using ScratchDotNet.Core.StageObjects;
using System.Drawing;

namespace ScratchDotNet.Core.Blocks.Motion;

internal static class MotionHelpers
{
    /// <summary>
    /// Evaluating a target as <see cref="SpecialTarget"/> or figure name to get the position
    /// </summary>
    /// <param name="target">The target string</param>
    /// <param name="context">The execution context</param>
    public static (double x, double y) GetTargetPosition(string target, ScriptExecutorContext context, ILogger logger)
    {
        switch (target)
        {
            case "_random_":
                return GenerateRandomPosition();
            case "_mouse_":
                Point mousePosition;
                try
                {
                    if (context.ServicesProvider.GetService<IInputProviderService>() is not IInputProviderService inputProvider)
                    {
                        logger.LogWarning("Could not find any registered service that implements {provider}.", nameof(IInputProviderService));
                        break;
                    }

                    mousePosition = inputProvider.GetMousePosition();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error happened while determining mouse position");
                    break;
                }

                return (mousePosition.X, mousePosition.Y);
            default:
                IFigure? targetFigure = context.Figures.FirstOrDefault(f => f.Name == target);
                if (targetFigure is null)
                {
                    logger.LogError("Cant find figure {figure} on stage", targetFigure);
                    break;
                }

                return (targetFigure.X, targetFigure.Y);
        }

        return (0, 0);
    }

    /// <summary>
    /// Determines the target string of a special target
    /// </summary>
    /// <param name="target">The target</param>
    /// <returns>The string</returns>
    public static string GetTargetString(SpecialTarget target)
    {
        return target switch
        {
            SpecialTarget.Random => "_random_",
            SpecialTarget.Mouse => "_mouse_",
            _ => throw new NotSupportedException("The specified special target isn't supported.")
        };
    }

    private static (double x, double y) GenerateRandomPosition()
    {
        int rndX = Random.Shared.Next(-250, 250);
        int rndY = Random.Shared.Next(-190, 190);
        return (rndX, rndY);
    }
}
