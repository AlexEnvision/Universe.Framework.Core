//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System;
using System.Collections.Generic;

namespace Universe.Algorithm.Tokenizer
{
    /// <summary>
    ///     Извлечение токенов из строки текста.
	///     Extract tokens from a string of text.
	/// <author>Alex Envision</author>
	/// </summary>
	public class TextTokenizer
	{
		private readonly string _data;

		private int _tokenBegin;
		private int _tokenEnd;
		private int _pos;

        /// <summary>
        ///     Создаёт экземпляр для токенизации текста из заданного <see cref="string"/>.
        ///     Creates an instance to tokenize text from the given <see cref="string"/>.
        /// </summary>
        /// <param name="data"></param>
        public TextTokenizer(string data)
		{
            _data = data ?? throw new ArgumentNullException(nameof(data));
		}

        /// <summary>
        ///     Получает или задает значение, указывающее типы маркеров, которые должны быть выпущены.
        ///     Gets or sets a value indicating the types of tokens to be emitted.
        /// </summary>
        public Tokens EmitTypes { get; set; } = Tokens.Word;

        /// <summary>
        ///     Отслеживает текущий тип маркера по мере обработки текста.
        ///     Tracks the current token type as the text is processed.
        /// </summary>
        public Tokens CurrentType { get; private set; } = Tokens.None;

        /// <summary>
        ///     Указывает, соответствует ли текущий тип токена любому из заданных типов.
        ///     Indicates whether the current token type matches any of the given types.
        /// </summary>
        /// <param name="tokenTypes">
        ///     Перечисление Flags, описывающее типы токенов для сравнения с текущим типом (CurrentType).
        ///     Flags enum describing the token types to compare with the CurrentType.</param>
        /// <returns>
        ///     Логическое значение, указывающее, соответствует ли текущий тип (CurrentType) любому из типов параметра.
        ///     A Boolean indicating if the CurrentType matches any of the types from the parameter.
        /// </returns>
        private bool Currently(Tokens tokenTypes)
		{
			return (CurrentType & tokenTypes) > 0;
		}

        /// <summary>
        ///     Создает токен (то есть строку) из текущего состояния токенизатора.
        ///     Produces a token (i.e. a string) from the current state of the tokenizer.
        /// </summary>
        /// <returns>
        ///     Последний токен, идентифицированный токенизатором.
        ///     The latest token identified by the tokenizer.
        /// </returns>
        private string CurrentToken()
		{
			return _data.Substring(_tokenBegin, _tokenEnd - _tokenBegin + 1);
		}

        /// <summary>
        ///     Обновление индексов начала/конца/позиции и CurrentType, по мере необходимости.
        ///     Update begin/end/pos indices and the CurrentType if necessary.
        /// </summary>
        /// <param name="nextType">
        ///     Целевой тип для перехода после создания или пропуска текущего токена.
        ///     Target type for transition after the current token is emitted or skipped.
        /// </param>
        /// <param name="compatibleTypes">
        ///     Типы токенов, в которых токен не сбрасывается при переходе от этих типов.
        ///     Token type(s) where the token isn't reset when transitioning from these types.
        /// </param>
        /// <param name="typesToEmit">
        ///     Испускать вместо перехода, если в настоящее время используется один из этих типов токенов.
        ///     Emit instead of transitioning if currently one of these token types.
        /// </param>
        /// <returns>
        ///     Истинно, если токен должен быть выпущен вместо перехода.
        ///     True, if the token is to be emitted instead of transitioning.
        /// </returns>
        private bool Transition(Tokens nextType, Tokens compatibleTypes, Tokens typesToEmit)
		{
			if (Currently(typesToEmit))
			{
				return true;
			}

			if (!Currently(compatibleTypes))
			{
				_tokenBegin = _pos;
				CurrentType = nextType;
			}

			_tokenEnd = _pos;
			++_pos;
			return false;
		}

        /// <summary>
        ///     Извлекает следующий токен из последовательности.
        ///     Extract the next token from the sequence.
        /// </summary>
        /// <returns>
        ///     Строка, содержащая извлеченный токен.
        ///     A string containing the extracted token.
        /// </returns>
        private string NextToken()
		{
			bool emit = false;

            // Перебор каждого символа; обновить начальные/конечные индексы
            // и "текущий" тип токена на каждом шаге. Если текущий тип
            // изменился, вернуть последний токен. Метод произведёт выбор в том же месте,
            // где он остановился при повторном вызове.
            // Step through each character; update the begin/end indices
            // and "current" token type at each step. If the CurrentType
            // changes, return the latest token. The method will pick up
            // where it left off when called again.
            while (_pos < _data.Length && !emit)
			{
				char ch = _data[_pos];

                // Слово и словоподобные символы.
                // Word and wordlike characters.
                if (Char.IsLetter(ch) || ch == '_' || ((ch == '\'' || ch == '’') && _pos + 1 < _data.Length && Char.IsLetter(_data[_pos+1])))
				{
					emit = Transition(Tokens.Word, Tokens.Word | Tokens.Number, Tokens.Newline | Tokens.Space | Tokens.Symbol);
				}
                // Числовые символы.
                // Number characters.
                else if (Char.IsDigit(ch) || (Currently(Tokens.Number) && (ch == '.' || ch == ',')))
				{
					emit = Transition(Tokens.Number, Tokens.Word | Tokens.Number, Tokens.Newline | Tokens.Space | Tokens.Symbol);
				}
                // Пробельные символы, не являющиеся новой строкой.
                // Non-Newline Whitespace characters.
                else if (Char.IsWhiteSpace(ch) && ch != '\r' && ch != '\n')
				{
					emit = Transition(Tokens.Space, Tokens.Space, Tokens.Newline | Tokens.Symbol | Tokens.Word);
				}
                // Символы новой строки.
                // Newline characters.
                else if (ch == '\r' || ch == '\n')
				{
					emit = Transition(Tokens.Newline, Tokens.Newline, Tokens.Number | Tokens.Space | Tokens.Symbol | Tokens.Word);
				}
                // Символы символов.
                // Symbol characters.
                else
                {
					emit = Transition(Tokens.Symbol, Tokens.Symbol, Tokens.Newline | Tokens.Number | Tokens.Space | Tokens.Word);
				}
			}

			return CurrentToken();
		}

        /// <summary>
        ///     Перечисляет токены, типы которых соответствуют разрешенным EmitTypes.
        ///     Enumerate the tokens whose types match those permitted by EmitTypes.
        /// </summary>
        /// <returns>
        ///     Перечисление токенов (то есть строк).
        ///     An enumeration of tokens (i.e. strings).
        /// </returns>
        public IEnumerable<string> Tokenize()
		{
			while (_pos < _data.Length)
			{
				string token = NextToken();

				if (Currently(EmitTypes))
				{
					yield return token;
				}

				CurrentType = Tokens.None;
			}
		}
	}
}
