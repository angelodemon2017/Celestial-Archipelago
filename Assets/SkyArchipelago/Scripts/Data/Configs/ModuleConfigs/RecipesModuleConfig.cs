using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Recipes Module Config")]
public class RecipesModuleConfig : ModuleConfig
{
    public override CtxFlag KeyFlag => CtxFlag.HaveRecipe;

    public List<RecipeConfig> AvailableRecipes;
}