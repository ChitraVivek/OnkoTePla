using System;
using System.Collections.Generic;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Server.Visualization.SampleDataGenerators
{
	public class PatientNameGenerator
	{
		private readonly Random rand;

		private readonly IReadOnlyList<string> firstNames;
		private readonly IReadOnlyList<string> surNames; 

		public PatientNameGenerator()
		{
			rand = new Random();

			firstNames = FirstNames();
			surNames   = SurNames();
		}

		public Patient NewPatient()
		{
			
			var firstNamesCount = firstNames.Count;
			var surNamesCount = surNames.Count;

			
		    return new Patient(firstNames[rand.Next(firstNamesCount-1)] + " " + surNames[rand.Next(surNamesCount-1)],
							   new Date((byte)rand.Next(1, 28),
							   			(byte)rand.Next(1, 13),
							   			(ushort)rand.Next(1936, 1992)),
							   GetRandomBoolValue(),
							   Guid.NewGuid(),
							   "noExternalID",
							   false);						
		}

		private bool GetRandomBoolValue ()
		{
			return rand.NextDouble() > 0.2;
		}

		private static IReadOnlyList<string> FirstNames ()
		{
			return new List<string>
			{
				"Aaron",     "Adam",      "Alan",     "Albert",  "Alice",      "Amanda",    "Amy",          "Andrea",   "Andrew",   "Angela",
				"Ann",       "Anna",      "Anne",     "Annie",   "Anthony",    "Arthur",    "Barbara",      "Benjamin", "Beverly",  "Billy",
				"Bobby",     "Bonnie",    "Brandon",  "Brenda",  "Brian",      "Bruce",     "Carl",         "Carlos",   "Carol",    "Carolyn",
				"Catherine", "Charles",   "Cheryl",   "Chris",   "Christina",  "Christine", "Christopher",  "Clarence", "Craig",    "Cynthia",
				"Daniel",    "David",     "Deborah",  "Debra",   "Denise",     "Dennis",    "Diana",        "Diane",    "Donald",   "Doris",
				"Dorothy",   "Douglas",   "Earl",     "Edward",  "Elizabeth",  "Emily",     "Eric",         "Ernest",   "Eugene",   "Evelyn",
				"Frances",   "Frank",     "Fred",     "Gary",    "George",     "Gerald",    "Gloria",       "Gregory",  "Harold",   "Harry",
				"Henry",     "Howard",    "Irene",    "Jack",    "Jacqueline", "James",     "Jane",         "Janet",    "Janice",   "Jason",
				"Jean",      "Jeffrey",   "Jennifer", "Jeremy",  "Jerry",      "Jesse",     "Jessica",      "Jimmy",    "Joan",     "Joe",
				"Jonathan",  "Joseph",    "Joshua",   "Joyce",   "Juan",       "Judith",    "Judy",         "Julia",    "Julie",    "Justin",
				"Karen",     "Katherine", "Kathleen", "Kathryn", "Kathy",      "Keith",     "Kelly",        "Kenneth",  "Kimberly", "Larry",
				"Laura",     "Lawrence",  "Linda",    "Lisa",    "Lois",       "Lori",      "Louis",        "Louise",   "Margaret", "Maria",
				"Marie",     "Marilyn",   "Mark",     "Martha",  "Martin",     "Mary",      "Matthew",      "Melissa",  "Michael",  "Michelle",
				"Mildred",   "Nancy",     "Nicholas", "Nicole",  "Norma",      "Pamela",    "Patricia",     "Patrick",  "Paul",     "Paula",
				"Peter",     "Philip",    "Phillip",  "Phyllis", "Rachel",     "Ralph",     "Randy",        "Raymond",  "Rebecca",  "Robert",
				"Ronald",    "Rose",      "Roy",      "Ruby",    "Russell",    "Ruth",      "Ryan",         "Samuel",   "Sandra",   "Sara",
				"Sarah",     "Scott",     "Sean",     "Sharon",  "Shawn",      "Shirley",   "Stephanie",    "Stephen",  "Steve",    "Steven",
				"Susan",     "Teresa",    "Terry",    "Theresa", "Thomas",     "Timothy",   "Tina",         "Todd",     "Victor",   "Walter",
				"Wanda",     "Wayne",     "William",  "Willie"
			};
		}

		private static IReadOnlyList<string> SurNames ()
		{
			return new List<string>
			{
				"Adams",        "Alexander",    "Allen",    "Anderson", "Bailey",       "Baker",        "Barnes",       "Bell",     "Bennett",  "Brooks",
				"Brown",        "Bryant",       "Butler",   "Campbell", "Carter",       "Clark",        "Coleman",      "Collins",  "Cook",     "Cooper",
				"Cox",          "Davis",        "Diaz",     "Edwards",  "Evans",        "Flores",       "Foster",       "Garcia",   "Gonzales", "Gonzalez",
				"Gray",         "Green",        "Griffin",  "Hall",     "Harris",       "Henderson",    "Hernandez",    "Hill",     "Howard",   "Hughes",
				"Jackson",      "James",        "Jenkins",  "Johnson",  "Jones",        "Kelly",        "King",         "Lee",      "Lewis",    "Long",
				"Lopez",        "Martin",       "Martinez", "Miller",   "Mitchell",     "Moore",        "Morgan",       "Morris",   "Murphy",   "Nelson",
				"Parker",       "Patterson",    "Perez",    "Perry",    "Peterson",     "Phillips",     "Powell",       "Price",    "Ramirez",  "Reed",
				"Richardson",   "Rivera",       "Roberts",  "Robinson", "Rodriguez",    "Rogers",       "Ross",         "Russell",  "Sanchez",  "Sanders",
				"Scott",        "Simmons",      "Smith",    "Stewart",  "Taylor",       "Thomas",       "Thompson",     "Torres",   "Turner",   "Walker",
				"Ward",         "Washington",   "Watson",   "White",    "Williams",     "Wilson",       "Wood",         "Wright",   "Young"
			};
		}
	}
}
