using System.Collections.Generic;
using System.Text.Json;
using Godot;

public partial class AddFlashcard : Control
{
	private Button Add;
	private Button AddCategory;

	public HomeScreen Home;
	public bool IsEditing = false;
	public FlashCard EditingCard;

	public OptionButton CategoryDropDown;
	public TextEdit Question;
	public TextEdit Answer;

	private PackedScene AddCategoryScene;
	
	public override void _Ready()
	{
		Add = GetNode<Button>("Add");
		AddCategory = GetNode<Button>("AddCategory");
		CategoryDropDown = GetNode<OptionButton>("CategoryInput");
		Question = GetNode<TextEdit>("QuestionInput");
		Answer = GetNode<TextEdit>("AnswerInput");

		AddCategoryScene = GD.Load<PackedScene>("res://Scenes/AddCategory.tscn");

		LoadCategories();

		Add.Pressed += OnAddPressed;
		AddCategory.Pressed += OnAddCategoryPressed;
	}

	public void OnAddPressed()
	{
		if (string.IsNullOrWhiteSpace(Question.Text) || string.IsNullOrWhiteSpace(Answer.Text))
		{
			return;
		}
		
		MakeCard();
	}
	public void MakeCard()
	{
		if (IsEditing)
    	{
        	EditingCard.Question = Question.Text;
        	EditingCard.Answer = Answer.Text;
        	EditingCard.Category = CategoryDropDown.GetItemText(CategoryDropDown.Selected);
    	}
    	else
    	{
        	FlashCard card = new FlashCard();

        	card.Question = Question.Text;
        	card.Answer = Answer.Text;
        	card.Category = CategoryDropDown.GetItemText(CategoryDropDown.Selected);

        	Home.Flashcards.Add(card);
    	}

    	Home.SaveData();
    	Home.FilterCards();

    	QueueFree();
	}

	public void OnAddCategoryPressed()
	{
		AddCategory CategoryPopup = AddCategoryScene.Instantiate<AddCategory>();

		CategoryPopup.FlashcardScreen = this;

		AddChild(CategoryPopup);
	}

	public void LoadCategories()
	{
		if (!FileAccess.FileExists("user://categories.json"))
		{
			return;
		}
		using var file = FileAccess.Open("user://categories.json", FileAccess.ModeFlags.Read);

		string json = file.GetAsText();

		Home.Categories = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

		CategoryDropDown.Clear();
		foreach (string Category in Home.Categories)
		{
			CategoryDropDown.AddItem(Category);
		}
	}
}
