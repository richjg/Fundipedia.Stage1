using System;
using System.ComponentModel.DataAnnotations;

namespace Fundipedia.TechnicalInterview.Model.Supplier;

public class Phone
{
    /// <summary>
    /// Gets or sets the phone id
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    [Required]
    [RegularExpression("^[0-9]{1,10}$",ErrorMessage = "Phone number is invalid. Must be only numbers and a max of 10 digits")]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the email is the preferred one or not
    /// </summary>
    public bool IsPreferred { get; set; }
}