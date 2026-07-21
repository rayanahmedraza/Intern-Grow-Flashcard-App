using Godot;

public partial class AddCategory : Control
{
	private Button Add;
	private TextEdit CategoryName;

	public AddFlashcard FlashcardScreen;
	public override void _Ready()
	{
		Add = GetNode<Button>("AddCategory");
		CategoryName = GetNode<TextEdit>("CategoryName");

		Add.Pressed += OnAddPressed;
	}

	public void OnAddPressed()
	{
    string category = CategoryName.Text.Trim();

	if (string.IsNullOrWhiteSpace(category) || FlashcardScreen.Home.Categories.Contains(category))
	{
		return;
	}

    FlashcardScreen.Home.Categories.Add(category);

    FlashcardScreen.Home.SaveCategories();
	FlashcardScreen.Home.LoadCategories();
    FlashcardScreen.LoadCategories();

    QueueFree();
	}
}
