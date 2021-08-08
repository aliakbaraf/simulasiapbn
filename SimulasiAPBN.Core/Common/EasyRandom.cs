namespace SimulasiAPBN.Core.Common
{
	public static class EasyRandom
	{
		private const string LowerCases = "abcdefghijklmnopqursuvwxyz";
		private const string UpperCases = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string Numbers = "123456789";
		private const string SpecialCharacters = @"!@£$%^&*()#€";
		
		public static string GeneratePassword(int passwordSize, 
			bool useLowerCase = true, bool useUpperCase = true, bool useNumber = true, bool useSpecialCharacter = true)
		{
			var random = new System.Random();
			var characterSet = string.Empty;
			var password = new char[passwordSize];

			if (useLowerCase)
			{
				characterSet += LowerCases;
			}
			
			if (useUpperCase)
			{
				characterSet += UpperCases;
			}
			
			if (useNumber) 
			{
				characterSet += Numbers;
			}

			if (useSpecialCharacter)
			{
				characterSet += SpecialCharacters;
			}

			for (var counter = 0; counter < passwordSize; counter++)
			{
				password[counter] = characterSet[random.Next(characterSet.Length - 1)];
			}

			return string.Join(null, password);
		}
	}
}