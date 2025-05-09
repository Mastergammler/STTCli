using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
///   Basic list item for friction list, problem list, todo list etc
/// </summary>
public class ListItem
{
    [NotMapped]
    public bool IsFinished => Finished is not null;

    public long Id { set; get; }
    public string Name { set; get; }
    public DateTime Created { set; get; }
    public DateTime? Finished { set; get; }
    public DateTime? Deadline { set; get; }

    // Bitset of tags -> up to 64 values
    public long Tags { set; get; }

    //TODO: Project?

    //TODO: Different types of tags? Is this necessary?

    //TODO: State tag? Do i want to differentiate between state & Category tags?

    //TODO: Priority: I would like this to sort by it
    //- the general question becomes, do i want to be super universal -> tag by everything
    //- or do i want to have different categories?
    //- tags are generally not good for sorting, you can do a tag order, but it's a bit akward
}