using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Xunit;

namespace RecipeBox
{
  public class RecipeTest : IDisposable
  {
    public RecipeTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=recipebox_test;Integrated Security=SSPI;";
    }
    public void Dispose()
    {
      Recipe.DeleteAll();
      Tag.DeleteAll();
    }

    [Fact]
    public void GetAll_DatabaseEmptyAtFirst_Empty()
    {
      int result = Recipe.GetAll().Count;

      Assert.Equal(0, result);
    }

    [Fact]
    public void Equals_EntryIsEqual_true()
    {
      Recipe recipe1 = new Recipe ("Spaghetti", "<Pasta, <Marinara Sauce", "Boil water, cook pasta, strain pasta, add sauce", 5, "30 mins");
      Recipe recipe2 = new Recipe ("Spaghetti", "<Pasta, <Marinara Sauce", "Boil water, cook pasta, strain pasta, add sauce", 5, "30 mins");

      Assert.Equal(recipe1, recipe2);
    }

    [Fact]
    public void Save_SaveToDatabase_Save()
    {
      Recipe testRecipe = new Recipe ("Chicken Soup", "<Chicken, <Chicken Broth", "Boil broth, cook chicken, put chicken in broth", 4, "30 mins");
      testRecipe.Save();

      List<Recipe> result = Recipe.GetAll();
      List<Recipe> verify = new List<Recipe>{testRecipe};

      Assert.Equal(verify, result);
    }

    [Fact]
    public void Save_SaveToDatabase_SaveWithId()
    {
      Recipe testRecipe = new Recipe ("Chicken Soup", "<Chicken, <Chicken Broth", "Boil broth, cook chicken, put chicken in broth", 4, "30 mins");
      testRecipe.Save();
      Recipe savedRecipe = Recipe.GetAll()[0];

      int output = savedRecipe.GetId();
      int expected = testRecipe.GetId();

      Assert.Equal(expected, output);
    }

    [Fact]
    public void AddTag_OneRecipe_TagAddedToJoinTable()
    {
      //Arrange
      Recipe testRecipe = new Recipe ("Spaghetti", "<Pasta, <Marinara Sauce", "Boil water, cook pasta, strain pasta, add sauce", 5, "30 mins");
      testRecipe.Save();
      Tag testTag = new Tag("Japanese");
      testTag.Save();
      testRecipe.AddTag(testTag);

      //Act
      List<Tag> output = testRecipe.GetTag();
      List<Tag> verify = new List<Tag>{testTag};

      //Assert
      Assert.Equal(verify, output);
    }

    [Fact]
    public void Test_DeleteRecipe_DeleteRecipeFromDatabase()
    {
      //Arrange
      Recipe testRecipe = new Recipe("Spaghetti", "<Pasta, <Marinara Sauce", "Boil water, cook pasta, strain pasta, add sauce", 5, "30 mins");

      //Act
      testRecipe.Save();
      testRecipe.Delete();

      //Assert
      List<Recipe> expectedResult = new List<Recipe>{};
      List<Recipe> actualResult = Recipe.GetAll();

      Assert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public void Test_Update_UpdateRecipeInDatabase()
    {
      Recipe testRecipe = new Recipe("Spaghetti", "<Pasta, <Marinara Sauce", "Boil water, cook pasta, strain pasta, add sauce", 5, "30 mins");
      testRecipe.Save();

      string newRecipeName ="Chicken Soup";
      string newIngredients = "<Chicken, <Chicken Broth";
      string newInstructions = "Boil broth, cook chicken, add chicken to broth";
      int newRate = 4;
      string newTime = "30 mins";

      testRecipe.Update(newRecipeName, newIngredients, newInstructions, newRate, newTime);
      Recipe actualResult = testRecipe;
      Recipe expectedResult = new Recipe(newRecipeName, newIngredients, newInstructions, newRate, newTime, testRecipe.GetId());

      Assert.Equal(expectedResult,actualResult);
    }
  }
}
