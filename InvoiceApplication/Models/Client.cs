using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApplication.Models
{
    /// <summary>
    /// Represents a client to whom an invoice can be issued.
    /// </summary>
    public class Client
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } // Primary key
        public string Name { get; set; } // Client's name
        public string Email { get; set; } // Client's email
        public string Phone { get; set; } // Client's phone number
        public string Address { get; set; } // Client's address

        public Client(string name, string email, string phone, string address) {
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
        }

        public override string ToString()
        {
            return Name; // Display the client's name in UI components
        }
    }
}

