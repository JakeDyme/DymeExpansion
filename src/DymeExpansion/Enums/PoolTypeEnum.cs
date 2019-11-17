
namespace DymeExpansion.Core.Enums
{
  public enum WhenItemsRunOutThen
  {
    startAtTheBeginningAgain,
    stopPickingItems
  }
  public enum PickEachItem
  {
    atLeastOnce,
    onlyIfNeeded
  }
  public enum PickItemsFromList
  {
    fromBeginningToEnd,
    randomly
  }
}
