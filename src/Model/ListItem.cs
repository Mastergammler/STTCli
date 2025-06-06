using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
///   Basic list item for friction list, problem list, todo list etc
/// </summary>
public class ListItem
{
    [NotMapped]
    public bool IsFinished => Finished is not null;

    [NotMapped]
    public bool IsLeaf => Level == 0;

    public long Id { set; get; }
    public string Name { set; get; }
    public DateTime Created { set; get; }
    public DateTime? Finished { set; get; }
    public DateTime? Deadline { set; get; }

    // Bitset of tags -> up to 64 values
    public long Tags { set; get; }

    // Parent node of this item
    public ListItem? Parent { set; get; }

    // Level of this node, where 0 is the leaf, 1 is the project above,
    // 2 would be project of projects etc
    public int Level { set; get; }
}