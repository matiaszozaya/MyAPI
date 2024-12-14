﻿namespace MyAPI.Models
{
	public class RegisterModel
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string DocumentId { get; set; }
		public string Province { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public string Role { get; set; }
	}
}