using Godot;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json;

public partial class HomeScreen : Control
{
    private Button Next;
    private Button Prev;
    private Button AddFlashcard;
    private Button DeleteFlashcard;
    private Button EditFlashcard;
    private Button ToggleAnswer;
    private Label Question;
    private Label Answer;
    private Label Category;

    public List<FlashCard> Flashcards = new();
    public List<FlashCard> FilteredCards = new();
    public List<string> Categories = new();
    public int CurrentIndex = 0;

    private PackedScene AddScene;

    private OptionButton CategorySelect;
    private Boolean ShowAnswer = false;

    public override void _Ready()
    {
        Next = GetNode<Button>("Next");
        Prev = GetNode<Button>("Prev");
        AddFlashcard = GetNode<Button>("AddFlashcard");
        DeleteFlashcard = GetNode<Button>("DeleteFlashcard");
        EditFlashcard = GetNode<Button>("EditFlashcard");
        ToggleAnswer = GetNode<Button>("ToggleAnswer");
        Question = GetNode<Label>("Question");
        Answer = GetNode<Label>("Answer");
        Category = GetNode<Label>("Category");
        CategorySelect = GetNode<OptionButton>("CategorySelect");
        AddScene = GD.Load<PackedScene>("res://Scenes/AddFlashcard.tscn");

        Next.Pressed += OnNextPressed;
        Prev.Pressed += OnPrevPressed;
        AddFlashcard.Pressed += OnAddFlashcardPressed;
        DeleteFlashcard.Pressed += OnDeleteFlashcardPressed;
        EditFlashcard.Pressed += OnEditFlashcardPressed;
        ToggleAnswer.Pressed += OnToggleAnswerPressed;
        CategorySelect.ItemSelected += OnCategoryChanged;

        GD.Print(ShowAnswer);
        

        LoadData();
        LoadCategories();
        if (CategorySelect.ItemCount > 0)
        {
            CategorySelect.Select(0);
        }
        FilterCards();
            
    }


    public void SaveData()
    {
        string json = JsonSerializer.Serialize(Flashcards);

        using var file = FileAccess.Open("user://flashcards.json", FileAccess.ModeFlags.Write);
        file.StoreString(json);
    }
    public void DeleteData()
    {
        if (FilteredCards.Count == 0)
        {
            return;
        }
        FlashCard card = FilteredCards[CurrentIndex];
        Flashcards.Remove(card);
        SaveData();
        LoadData();
        FilterCards();
    }
    public void OnDeleteFlashcardPressed()
    {
        DeleteData();
    }
    public void OnEditFlashcardPressed()
    {
        AddFlashcard popup = OpenWindow();

        popup.IsEditing = true;
        popup.EditingCard = FilteredCards[CurrentIndex];

        popup.Question.Text = popup.EditingCard.Question;
        popup.Answer.Text = popup.EditingCard.Answer;
    }

    public void LoadData()
    {
        if (!FileAccess.FileExists("user://flashcards.json"))
        {
            return;
        }
        using var file = FileAccess.Open("user://flashcards.json", FileAccess.ModeFlags.Read);

        string json = file.GetAsText();

        Flashcards = JsonSerializer.Deserialize<List<FlashCard>>(json) ?? new List<FlashCard>();

    }

    public void OnNextPressed()
    {
        if (CurrentIndex < FilteredCards.Count - 1)
        {
            CurrentIndex++;
            DisplayCurrentCard();
        }
    }

    public void OnPrevPressed()
    {
        if (CurrentIndex > 0)
        {
            CurrentIndex--;
            DisplayCurrentCard();
        }
    }

    public void OnAddFlashcardPressed()
    {
        
        AddFlashcard FlashcardPopup = AddScene.Instantiate<AddFlashcard>();

        FlashcardPopup.Home = this;

        AddChild(FlashcardPopup);
    }
    public AddFlashcard OpenWindow()
    {
        AddFlashcard FlashcardPopup = AddScene.Instantiate<AddFlashcard>();

        FlashcardPopup.Home = this;

        AddChild(FlashcardPopup);

        return FlashcardPopup;
    }

    public void DisplayCurrentCard()
    {
        if (FilteredCards.Count == 0)
        {
            return;
        }
        Category.Text = FilteredCards[CurrentIndex].Category;
        Question.Text = FilteredCards[CurrentIndex].Question;
        ShowAnswer = false;
        Answer.Text = "";
    }

    public void SaveCategories()
    {
        string json = JsonSerializer.Serialize(Categories);
    	using var file = FileAccess.Open("user://categories.json", FileAccess.ModeFlags.Write);
		file.StoreString(json);
    }
    public void LoadCategories()
	{
		if (!FileAccess.FileExists("user://categories.json"))
		{
			return;
		}
		using var file = FileAccess.Open("user://categories.json", FileAccess.ModeFlags.Read);

		string json = file.GetAsText();

		Categories = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        
		CategorySelect.Clear();
		foreach (string Category in Categories)
		{
			CategorySelect.AddItem(Category);
		}
	}

    public void FilterCards()
    {
        if (CategorySelect.Selected < 0)
        {
            return;
        }

        FilteredCards.Clear();
        string SelectedCategory = CategorySelect.GetItemText(CategorySelect.Selected);

        foreach (FlashCard card in Flashcards)
        {
            if (card.Category == SelectedCategory)
            {
                FilteredCards.Add(card);
            }
        }
        CurrentIndex = 0;
        if (FilteredCards.Count <= 0)
        {
            Answer.Text = "";
            Question.Text = "                 No Flashcards exist for this Category";
            Category.Text = "";
        }
        else
        {
            DisplayCurrentCard();
        }
        
    }
    public void OnCategoryChanged(long index)
    {
        FilterCards();
    }
    public void OnToggleAnswerPressed()
    {
        GD.Print(ShowAnswer);
        ShowAnswer = !ShowAnswer;

        if (ShowAnswer)
        {
            Answer.Text = FilteredCards[CurrentIndex].Answer;
        }
        else
        {
            Answer.Text = "";
        }
    }
}

public class FlashCard
{
    public string Question{get; set;}
    public string Answer{get; set;}
    public string Category{get; set;}
}