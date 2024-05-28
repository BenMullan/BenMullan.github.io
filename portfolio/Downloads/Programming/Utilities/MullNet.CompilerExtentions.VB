#Region "ReadMe"

'MullNet CompilerExtentions
'--------------------------
'This File contains non-project-specific versions of ALL Extention Methods
'
'Use them by typing "Imports MullNet.CompilerExtentions" or "using MullNet.CompilerExtentions;" at the top of your file.
'Then simply call one of the Extention Methods on the relevant type E.g.
'	SomeString.MatchesRegEx("\d{5}")
'	SomeInt32.IsEven()
'	SomeBitmap.GetZoomedInCopy(0.4)
'
'If a Extentions for a particular Type are not needed, then remove the module for that type.
'E.g. Windows Forms extentions aren't needed for a Web Project, so either add a Project Referance to [System.Windows.Forms.dll], or delete the [ControlExtentions] Module in this file...
'All the Extention Methods will only naturally throw [CompilerExtentionException]s
'
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'Modules whose Extention Types need a specific DLL, have that DLL named above the Module in an XML Comment
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

#End Region

Namespace CompilerExtentions

	Public Module StringExtentions

		REM 09	=	Horisontal Tab
		REM 10	=	Line Feed
		REM	11	=	Vertical Tab
		REM 12	=	Form Feed
		REM 13	=	Carridge Return
		REM 32	=	Space
		Public ReadOnly WhitespaceChars As Char() = {Chr(9I), Chr(10I), Chr(11I), Chr(12I), Chr(13I), Chr(32I)}

		''' <summary>
		''' (MullNet CompilerExtention) Splits the String whenever one of the _CharsAtWhichToSplitString occours, but the _CharAtWhichToSplitString stays in each New String
		''' <example>
		''' "Hello, World; Today" and {","c, ";"c} Becomes {"Hello", ", World", "; Today"}
		''' </example>
		''' </summary>
		''' <returns>The _InputString split at each occourance of a _CharAtWhichToSplitString</returns>
		''' <remarks></remarks>
		<System.Runtime.CompilerServices.Extension()>
		Public Function SplitAtWithoutRemoving(ByVal _InputString$, ByVal _CharsAtWhichToSplitString As Char()) As String()

			Dim _StringsToReturn As New List(Of String)()
			Dim _StringToAddAccumulator$ = String.Empty

			For Each _Char As Char In _InputString.ToCharArray()

				If _CharsAtWhichToSplitString.Contains(_Char) Then

					REM Only add the String to be returned, if it is not empty
					If (Not String.IsNullOrEmpty(_StringToAddAccumulator)) Then _StringsToReturn.Add(_StringToAddAccumulator)
					_StringToAddAccumulator = String.Empty

				Else
					REM The _Char isn't one to split the String at
				End If

				_StringToAddAccumulator &= _Char.ToString()

			Next

			REM Add the Remaining contents of the Accumulator to the output
			_StringsToReturn.Add(_StringToAddAccumulator)

			Return _StringsToReturn.ToArray()

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns a version of the string with any Spaces, Tabs, or LineBreak Characters which appear at the start of the String removed.
		''' </summary>
		''' <param name="_InputString">The String which may contain Leading Whitespace Characters</param>
		''' <returns>The String without the Leading (preceding) WhiteSpace Characters</returns>
		''' <remarks>Only returns the resultant output; does not modify the Input String. Chars removed (ASCII): 09, 10, 11, 12, 13, 32</remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function WithLeadingWhiteSpaceRemoved$(ByVal _InputString$)

			For Each _Char As Char In _InputString$.ToCharArray()

				'If the _Char is one of the ones we want to Remove...
				If WhitespaceChars.Contains(_Char) Then
					'...Then Remove it.
					_InputString.RemoveFirstCharacter()
					REM The above does not modify the origional argument passed to the function at the call, because it is passed as ByVal, so it's only modifying a copy thereof.
				Else
					'Otherwise, We must have hit the start of meaningful Characters in the String, so stop checking.
					'Simply return the String as it is, now that WhiteSpace has been removed.
					Return _InputString
				End If

			Next

			'We could end up here, if:
			'	the Entire _InputString$ was all WhiteSpace,
			'	Or, the _InputString was a String::Empty when passed to the Function,
			'So retrun it, even though it's empty.
			Return _InputString

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns a version of the string with any Spaces, Tabs, or LineBreak Characters which appear at the end of the String removed.
		''' </summary>
		''' <param name="_InputString">The String which may contain Trailing Whitespace Characters</param>
		''' <returns>The String without the Trailing (following) WhiteSpace Characters</returns>
		''' <remarks>Only returns the resultant output; does not modify the Input String. Chars removed (ASCII): 09, 10, 11, 12, 13, 32</remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function WithTrailingWhiteSpaceRemoved$(ByVal _InputString$)

			For Each _Char As Char In _InputString$.ToCharArray().Reverse()

				'If the _Char is one of the ones we want to Remove...
				If WhitespaceChars.Contains(_Char) Then
					'...Then Remove it.
					_InputString.RemoveLastCharacter()
					REM The above does not modify the origional argument passed to the function at the call, because it is passed as ByVal, so it's only modifying a copy thereof.
				Else
					'(REMEMBER: this is going from the last Char in the String to the first one)
					'Otherwise, We must have hit the start (end) of meaningful Characters in the String, so stop checking.
					'Simply return the String as it is, now that WhiteSpace has been removed.
					Return _InputString
				End If

			Next

			'We could end up here, if:
			'	the Entire _InputString$ was all WhiteSpace,
			'	Or, the _InputString was a String::Empty when passed to the Function,
			'So retrun it, even though it's empty.
			Return _InputString

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Removes the First Character from the String
		''' </summary>
		''' <param name="_InputString"></param>
		''' <returns></returns>
		''' <remarks>Both Modifies the Parameter, and returns the post-computation Result</remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function RemoveFirstCharacter$(ByRef _InputString$)

			'If the String is Empty, we wouldn't be able to Remove the FirstChar
			If _InputString.IsEmpty() Then Throw New CompilerExtentionException("The First Character could not be removed from the String, because the String is empty.")

			'If we've gotten to here, the String must have at least 1 Character, so Remove the FirstChar
			_InputString = _InputString.Remove(0I, 1I)
			Return _InputString

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Removes the Last Character from the String
		''' </summary>
		''' <param name="_InputString"></param>
		''' <returns></returns>
		''' <remarks>Both Modifies the Parameter, and returns the post-computation Result</remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function RemoveLastCharacter$(ByRef _InputString$)

			'If the String is Empty, we wouldn't be able to Remove the LastChar
			If _InputString.IsEmpty() Then Throw New CompilerExtentionException("The Last Character could not be removed from the String, because the String is empty.")

			'If we've gotten to here, the String must have at least 1 Character, so Remove the LastChar
			_InputString = _InputString.Remove(_InputString.Length - 1, 1I)
			Return _InputString

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Evaluates weather or not the String is Empty. (WhiteSpace does not count as Empty)
		''' </summary>
		''' <param name="_InputString"></param>
		''' <returns>Retruns True if the String is Empty</returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function IsEmpty(ByVal _InputString$) As Boolean
			Return _InputString = Global.System.String.Empty
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Matches a String against a Regular Expression
		''' </summary>
		''' <param name="_StringToValidate"></param>
		''' <param name="_RegExPattern">The pattern to satisfy</param>
		''' <returns>Returns True if the RegExp is satisfied by the Input String.</returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function MatchesRegEx(ByVal _StringToValidate$, ByVal _RegExPattern$) As Boolean
			Return (New Global.System.Text.RegularExpressions.Regex([pattern]:=[_RegExPattern])).IsMatch(_StringToValidate)
		End Function

		''' <summary>(MullNet CompilerExtention) Returns in the form [_InputString$]</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function InSquares$(ByRef _InputString$)
			Return ("["c & _InputString & "]"c)
		End Function

		''' <summary>(MullNet CompilerExtention) Returns in the form {_InputString$}</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function InCurlies$(ByRef _InputString$)
			Return ("{"c & _InputString & "}"c)
		End Function

		''' <summary>(MullNet CompilerExtention) Returns in the form &lt;_InputString$&gt;</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function InPointies$(ByRef _InputString$)
			Return ("<"c & _InputString & ">"c)
		End Function

		''' <summary>(MullNet CompilerExtention) Returns in the form (_InputString$)</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function InBrackets$(ByRef _InputString$)
			Return ("("c & _InputString & ")"c)
		End Function

		''' <summary>(MullNet CompilerExtention) Returns a string containing any Digit Chars (0 to 9) which were found in the _InputString$</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function GetDigitChars$(ByRef _InputString$)
			Return New String(_InputString.Where(AddressOf Char.IsDigit).ToArray())
		End Function

		REM <FromGraphPictures>

		''' <summary>
		''' (MullNet CompilerExtention) Chops off Excess Characters from the end of the String if it's too long
		''' </summary>
		<Runtime.CompilerServices.Extension()>
		Public Function MakeNoLongerThan(ByVal _InputString$, ByVal _MaxLength As UInt32) As String
			Return If(_InputString.Length > _MaxLength, _InputString.Substring(0, CInt(_MaxLength)), _InputString)
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Checks that the Path is Valid, before returning a version of it with a backslach if it dosen't have one already.
		''' </summary>
		<Runtime.CompilerServices.Extension()>
		Public Function Clense(ByVal _InputString$, ByVal _AllowedChars As Char()) As String
			Return (New String(_InputString.ToCharArray().Where(AddressOf _AllowedChars.Contains).ToArray()))
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Checks that the Path is Valid, before returning a version of it with a backslach if it dosen't have one already.
		''' </summary>
		<Runtime.CompilerServices.Extension()>
		Public Function MakeValidFolderPath(ByVal _PotentialFolderPath$) As String
			If (IO.Path.GetInvalidPathChars().Any(AddressOf _PotentialFolderPath.Contains)) Then Throw (New CompilerExtentionException("The Folder Path could not be made valid because it contained at least one invalid character. It was: {" & _PotentialFolderPath & "}"c))
			Return If(_PotentialFolderPath.LastCharacter() = "\"c, _PotentialFolderPath, _PotentialFolderPath & "\"c)
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Modifies the Existing String, as well as returning it.
		''' </summary>
		''' <param name="_InputString"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function RemoveLineBreaksAndTabs$(ByRef _InputString$)

			REM 12	=	Form Feed
			REM 13	=	Carridge Return

			_InputString = _InputString.Replace(vbCr, "")
			_InputString = _InputString.Replace(vbLf, "")
			_InputString = _InputString.Replace(vbTab, "")

			Return _InputString

		End Function

		''' <summary>
		''' (Custom Extention) Modifies the Existing String, as well as returning it.
		''' </summary>
		''' <param name="_InputString"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function ReplaceLineBreaksAndTabsWithSpaces$(ByRef _InputString$)

			REM 12	=	Form Feed
			REM 13	=	Carridge Return

			_InputString = _InputString.Replace(vbCr, " ")
			_InputString = _InputString.Replace(vbLf, " ")
			_InputString = _InputString.Replace(vbTab, " ")

			Return _InputString

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns a new version of the string, which dosen't contain any of the disallowed Characters
		''' </summary>
		''' <param name="_InputString"></param>
		''' <param name="_DisallowedChars">The characters to remove from the String</param>
		''' <returns>The String, without these Characters in it</returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function RemoveAllOccourancesOf(ByVal _InputString$, ByVal ParamArray _DisallowedChars As Char()) As Global.System.String

			Dim _CleanedStringToReturn$ = String.Empty

			For Each _Char As Char In _InputString.ToCharArray()
				If (Not _DisallowedChars.Contains(_Char)) Then _CleanedStringToReturn &= _Char.ToString()
			Next

			Return _InputString

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Removes any Spaces, Tabs, or LineBreak Characters which appear at the start of the String
		''' </summary>
		''' <param name="_InputString">The String from which to Remove WhiteSpace</param>
		''' <returns>The String which has been modified</returns>
		''' <remarks>Not only modifies this method the Input String, but it also returns the resultant output. Chars removed (ASCII): 09, 10, 11, 12, 13, 32</remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function RemoveLeadingWhiteSpace$(ByRef _InputString$)

			REM 09	=	Horisontal Tab
			REM 10	=	Line Feed
			REM	11	=	Vertical Tab
			REM 12	=	Form Feed
			REM 13	=	Carridge Return
			REM 32	=	Space

			Dim _DisallowedChars As Char() = {Chr(9I), Chr(10I), Chr(11I), Chr(12I), Chr(13I), Chr(32I)}

			For Each _Char As Char In _InputString$.ToCharArray()

				'If the _Char is one of the ones we want to Remove...
				If _DisallowedChars.Contains(_Char) Then
					'...Then Remove it.
					_InputString.RemoveFirstCharacter()
				Else
					'Otherwise, We must have hit the start of meaningful Characters in the String, so stop checking.
					'Simply return the String as it is, now that WhiteSpace has been removed.
					Return _InputString
				End If

			Next

			'We could end up here, if:
			'	the Entire _InputString$ was all WhiteSpace,
			'	Or, the _InputString was a String::Empty when passed to the Function,
			'So retrun it, even though it's empty.
			Return _InputString

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Removes any Spaces, Tabs, or LineBreak Characters which appear at the end of the String
		''' </summary>
		''' <param name="_InputString">The String from which to Remove WhiteSpace</param>
		''' <returns>The String which has been modified</returns>
		''' <remarks>Not only modifies this method the Input String, but it also returns the resultant output. Chars removed (ASCII): 09, 10, 11, 12, 13, 32</remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function RemoveTrailingWhiteSpace$(ByRef _InputString$)

			REM 09	=	Horisontal Tab
			REM 10	=	Line Feed
			REM	11	=	Vertical Tab
			REM 12	=	Form Feed
			REM 13	=	Carridge Return
			REM 32	=	Space

			Dim _DisallowedChars As Char() = {Chr(9I), Chr(10I), Chr(11I), Chr(12I), Chr(13I), Chr(32I)}

			For Each _Char As Char In _InputString$.ToCharArray().Reverse()

				'If the _Char is one of the ones we want to Remove...
				If _DisallowedChars.Contains(_Char) Then
					'...Then Remove it.
					_InputString.RemoveLastCharacter()
				Else
					'(REMEMBER: this is going from the last Char in the String to the first one)
					'Otherwise, We must have hit the start of meaningful Characters in the String, so stop checking.
					'Simply return the String as it is, now that WhiteSpace has been removed.
					Return _InputString
				End If

			Next

			'We could end up here, if:
			'	the Entire _InputString$ was all WhiteSpace,
			'	Or, the _InputString was a String::Empty when passed to the Function,
			'So retrun it, even though it's empty.
			Return _InputString

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Removes the Last two Characters from the String
		''' </summary>
		''' <param name="_InputString"></param>
		''' <returns></returns>
		''' <remarks>Both Modifies the Parameter, and returns the post-computation Result</remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function RemoveLastTwoCharacters$(ByRef _InputString$)

			'If the String is Empty, we wouldn't be able to Remove the LastChar
			If _InputString.IsEmpty() Then Throw New CompilerExtentionException("The Last Character could not be removed from the String, because the String is empty.")

			'If we've gotten to here, the String must have at least 1 Character, so Remove the LastChar
			_InputString = _InputString.Remove(_InputString.Length - 1, 1I)

			REM Check if the input is now empty wegen the last removal
			If _InputString.IsEmpty() Then Throw New CompilerExtentionException("The Second To Last Character could not be removed from the String, because the String was empty after the removal of it's first character.")

			_InputString = _InputString.Remove(_InputString.Length - 1, 1I)

			Return _InputString

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns the First Character in the String
		''' </summary>
		''' <param name="_InputString"></param>
		''' <returns>Returns the First Character in the String</returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function FirstCharacter(ByRef _InputString$) As Char

			If _InputString.IsEmpty() Then Throw New CompilerExtentionException("The First Character of the String could not be found, because the String is empty.")
			Return _InputString.ToCharArray()(0)

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns the Last Character in the String
		''' </summary>
		''' <param name="_InputString"></param>
		''' <returns>Returns the Last Character in the String</returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function LastCharacter(ByRef _InputString$) As Char

			If _InputString.IsEmpty() Then Throw New CompilerExtentionException("The Last Character of the String could not be found, because the String is empty.")
			Return _InputString.ToCharArray()(_InputString.Length - 1)

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Encapsulates the current System.String Object with the _StringWithWhichToWrapInput$. This modifies the String Object from which this method is called, and returns the result too.
		''' </summary>
		''' <param name="_InputString">The String to Wrap</param>
		''' <param name="_StringWithWhichToWrapInput">The String to prepend and append the _InputString$ with</param>
		''' <returns>The Encapsulated String</returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function EncapsulateIn(ByRef _InputString$, ByVal _StringWithWhichToWrapInput$) As String
			_InputString = _StringWithWhichToWrapInput & _InputString & _StringWithWhichToWrapInput
			Return _InputString
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Converts a Mathematical Expression E.g. 5 + (2 - 6) into a Latex one, replacing Brackets and MultiplyBy Symbols
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function MathsExpressionToLatex(ByVal _MathsExpression$) As System.String

			Return _MathsExpression.Replace("(", "\left(").Replace(")", "\right)").Replace("*", "\cdot ")

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Converts a LaTeX Expression, to a Mathematical Expression E.g. 5 + (2 - 6).Brackets and MultiplyBy Symbols are replaced
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function LatexToMathsExpression(ByVal _Latex$) As System.String

			Return _Latex$.Replace("\left(", "(").Replace("\right)", ")").Replace("\cdot ", "*")

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Attempts to convert the String to a Decimal. Throws a CompilerExtentionException on failure to do so.
		''' </summary>
		''' <param name="_InputString"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function ToDecimal(ByVal _InputString$) As System.Decimal

			Try

				If _InputString.IsEmpty() Then Throw (New CompilerExtentionException("The String was empty for ToDecimal()"))
				If Not _InputString.MatchesRegEx("^\d+(\.\d+)?$") Then Throw (New CompilerExtentionException("The String did not match the Regular Expression for a Decimal"))

				Return CDec(_InputString)

			Catch _Ex As Exception When True
				Throw (New CompilerExtentionException("The following Exception was thrown upon attempting to convert the string """ & _InputString & """, into a Decimal: " & _Ex.Message, _Ex))
			End Try

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns the String encoded into Base64.
		''' </summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function ToASCIIBase64(ByVal _InputString$) As String

			Try

				If _InputString.IsEmpty() Then Throw (New CompilerExtentionException("The String was empty for ToASCIIBase64()"))

				Return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(_InputString))

			Catch _Ex As Exception When True
				Throw (New CompilerExtentionException("The following Exception was thrown upon attempting to convert get the ASCII Base64 for a String: " & _Ex.Message, _Ex))
			End Try

		End Function

		REM </FromGraphPictures>

	End Module

	Public Module NumericExtentions

#Region "Numeric Conversion Extention Methods"

		Const BytesInKB% = 1024I

		''' <summary>
		''' (MullNet CompilerExtention) Converts [A Number of Bytes] into [How many KB that would be]
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function BytesToIntegralKB(ByVal _NumberOfBytes As UInt64) As UInt64

			Return CULng(CLng(_NumberOfBytes) \ BytesInKB%)

		End Function

#End Region

		''' <summary>
		''' Evaluates weather or not the _Integer is Even, by BitWise ANDing it with 1
		''' </summary>
		''' <param name="_Integer"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		<System.Runtime.CompilerServices.Extension()>
		Public Function IsEven(ByVal _Integer%) As Boolean
			REM Bitwise AND it with 1, to check the value of the right-most bit
			Return (_Integer And 1) = 0
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns 105% of the existing Integer Value, Rounded to the nearest Integer if a Decimal would be produced.
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function Plus5Percent(ByRef _InputInteger%) As Int32

			Return CInt(_InputInteger * 1.05)

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Evaluates weather or not the _InputInteger is [Higher than the _LowerValue] AND [Lower than the _HigherValue]
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function IsBetween(ByRef _InputNumber&, ByVal _LowerValue&, ByVal _HigherValue&) As Boolean

			Return ((_InputNumber > _LowerValue) AndAlso (_InputNumber < _HigherValue))

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Evaluates weather or not the _InputInteger is [Higher than OR EQUAL TO the _LowerValue] AND [Lower than OR EQUAL TO the _HigherValue]
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function IsBetweenInclusive(ByRef _InputInt64&, ByVal _LowerValue&, ByVal _HigherValue&) As Boolean

			Return ((_InputInt64 >= _LowerValue) AndAlso (_InputInt64 <= _HigherValue))

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Evaluates weather or not the _Integer is at least _Amount.
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function IsAtLeast(ByVal _Integer%, ByVal _Amount%) As Boolean

			Return (_Integer >= _Amount)

		End Function

	End Module

	Public Module DateTimeExtentions

		''' <summary>(MullNet CompilerExtention) Gets a short Date and Time String suitable for Logs, in the form "21-07 12:00:08.625"</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function GetLogSerialisation(ByRef _DateTime As Global.System.DateTime) As String
			Return _DateTime.ToString("dd-MM HH:mm:ss.fff")
		End Function

	End Module

	''' <remarks>Needs [System.Windows.Forms.dll] and [PresentationCore.dll] and [PresentationFramework.dll] and [WindowsBase.dll] and [System.Xaml.dll]</remarks>
	Public Module ControlExtentions

		''' <summary>
		''' (MullNet CompilerExtention) Invokes the Control if required, and runs the specified _Action
		''' </summary>
		<System.Runtime.CompilerServices.Extension()> _
		Public Sub InvokeIfRequired(ByVal _Control As Windows.Forms.Control, ByVal _Action As Action)
			If _Control.InvokeRequired Then
				_Control.Invoke(_Action)
			Else
				_Action.Invoke()
			End If
		End Sub

		''' <summary>(MullNet CompilerExtention) Returns the Text from the RichTextBox</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function GetText(ByRef _RichTextBox As Windows.Controls.RichTextBox) As [String]
			Try
				Return New Windows.Documents.TextRange(_RichTextBox.Document.ContentStart, _RichTextBox.Document.ContentEnd).Text
			Catch _Ex As Exception : Throw New CompilerExtentionException("@GetText: " & _Ex.Message, _Ex) : End Try
		End Function

		''' <summary>(MullNet CompilerExtention) Returns the Item1=Line and Item2=Column of the Caret in the RichTextBox</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function GetCaretLineAndColumn(ByRef _RichTextBox As Windows.Controls.RichTextBox) As Tuple(Of Int32, Int32)
			Try

				'int lineNumber;
				'textBox.CaretPosition.GetLineStartPosition(-int.MaxValue, out lineNumber);
				'int columnNumber = richTextBox.CaretPosition.GetLineStartposition(0).GetOffsetToPosition(richTextBox.CaretPosition);
				'if (lineNumber == 0)
				'    columnNumber--;

				'statusBarLineColumn.Content = string.Format("Line {0}, Column {1}", -lineNumber + 1, columnNumber + 1);

				Dim _LineNumber%
				_RichTextBox.CaretPosition.GetLineStartPosition(-Int32.MaxValue, _LineNumber)

				Dim _ColumnNumber% = _RichTextBox.CaretPosition.GetLineStartPosition(0).GetOffsetToPosition(_RichTextBox.CaretPosition)
				If _LineNumber = 0 Then _ColumnNumber -= 1

				Return New Tuple(Of Int32, Int32)(-_LineNumber + 1, _ColumnNumber + 1)

			Catch _Ex As Exception : Throw New CompilerExtentionException("@GetCaretLineAndColumn (CompilerExtention): " & _Ex.Message, _Ex) : End Try
		End Function

	End Module

	Public Module CollectionTypeExtentions

		''' <summary>
		''' (MullNet CompilerExtention) Returns all elements up to (but excluding) the first element where the _Condition is met.  Throws a CompilerExtentionException if there is not at least one element which meets the condition.
		''' </summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function UpToButExcluding(Of _TElement)(ByRef _Array As _TElement(), ByVal _Condition As Func(Of _TElement, Boolean)) As _TElement()
			Try
				REM Without this check, _IndexOfLastWantedElement would still be 0 when we hit the Return, so we'd return only the first element
				If Not _Array.Any(_Condition) Then Throw New CompilerExtentionException("None of the Elements in the _Array matched the _Condition")

				REM The _Array could be Empty:
				If _Array.Length = 0 Then Return _Array

				Dim _IndexOfLastWantedElement As Int32 = 0

				For _ElementIndex As Int32 = 0 To (_Array.Length - 1) Step +1
					If _Condition.Invoke(_Array(CInt(_ElementIndex))) Then
						'The last wanted element was the previous one
						_IndexOfLastWantedElement = (_ElementIndex - 1) : Exit For
					End If
				Next

				Return _Array.Take(_IndexOfLastWantedElement + 1).ToArray()

			Catch _Ex As Exception : Throw New CompilerExtentionException("@UpToButExcluding (CompilerExtention): " & _Ex.Message, _Ex) : End Try
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns all elements up to (and including) the first element where the _Condition is met. Throws a CompilerExtentionException if there is not at least one element which meets the condition.
		''' </summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function UpToAndIncluding(Of _TElement)(ByRef _Array As _TElement(), ByVal _Condition As Func(Of _TElement, Boolean)) As _TElement()
			Try
				REM Without this check, _IndexOfLastWantedElement would still be 0 when we hit the Return, so we'd return only the first element
				If Not _Array.Any(_Condition) Then Throw New CompilerExtentionException("None of the Elements in the _Array matched the _Condition")

				REM The _Array could be Empty:
				If _Array.Length = 0 Then Return _Array

				Dim _IndexOfLastWantedElement As UInt32 = 0

				For _ElementIndex% = 0 To (_Array.Length - 1) Step +1
					If _Condition.Invoke(_Array(_ElementIndex)) Then

						'The last wanted element was the previous one
						_IndexOfLastWantedElement = CUInt(_ElementIndex) : Exit For

					End If
				Next

				Return _Array.Take(CInt(_IndexOfLastWantedElement + 1)).ToArray()

			Catch _Ex As Exception : Throw New CompilerExtentionException("@UpToAndIncluding (CompilerExtention): " & _Ex.Message, _Ex) : End Try
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns chunks of Elements split by any Element where the _Condition is met. None of the Elements which meet the _Condition will end up in the output groups
		''' </summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function SplitWhere(Of _TElement)(ByRef _Array As _TElement(), ByVal _Condition As Func(Of _TElement, Boolean)) As _TElement()()
			Try

				REM The _Array could be Empty:
				If _Array.Length = 0 Then Return (New List(Of _TElement())).ToArray()

				Dim _ChunksToReturn As New List(Of _TElement())()
				Dim _ChunkAccumulator As New List(Of _TElement)()

				For Each _Element As _TElement In _Array
					If _Condition.Invoke(_Element) Then	'The _Element is one to Split at
						_ChunksToReturn.Add(_ChunkAccumulator.ToArray()) : _ChunkAccumulator.Clear()
					Else 'The _Element is not one to Split at
						_ChunkAccumulator.Add(_Element)
					End If
				Next

				REM Finally, empty the _ChunkAcc. out
				_ChunksToReturn.Add(_ChunkAccumulator.ToArray())

				Return _ChunksToReturn.ToArray()

			Catch _Ex As Exception : Throw New CompilerExtentionException("@SplitWhere (CompilerExtention): " & _Ex.Message, _Ex) : End Try
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns a version of the _Array wherein all Elements in the _Array which meet the _Condition are replaced with the _Replacement
		''' </summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function ReplaceWhere(Of _TElement)(ByVal _Array As _TElement(), ByVal _Condition As Func(Of _TElement, Boolean), ByVal _Replacement As _TElement) As _TElement()
			Try

				For _Index% = 0 To (_Array.Length - 1) Step +1
					If _Condition.Invoke(_Array(_Index)) Then _Array(_Index) = _Replacement
				Next

				Return _Array

			Catch _Ex As Exception : Throw New CompilerExtentionException("@ReplaceWhere (CompilerExtention): " & _Ex.Message, _Ex) : End Try
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns the Index of the first element to satisfy the _Condition. Throws a CompilerExtentionException if no elements satisfy the _Condition.
		''' </summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function IndexOfFirstWhere(Of _TElement)(ByVal _Array As _TElement(), ByVal _Condition As Func(Of _TElement, Boolean)) As Int32
			Try

				If Not _Array.Any(_Condition) Then Throw New CompilerExtentionException("None of the Elements in the _Array matched the _Condition")

				For _Index% = 0 To (_Array.Length - 1) Step +1
					If _Condition.Invoke(_Array(_Index)) Then
						Return _Index
					End If
				Next

				Throw New CompilerExtentionException("This Exception should never be seen; At least one element in the _Array must have satisfied the Condition, and yet the a Return didn't occour herebefore.")

			Catch _Ex As Exception : Throw New CompilerExtentionException("@IndexOfFirstWhere (CompilerExtention): " & _Ex.Message, _Ex) : End Try
		End Function

		''' <summary>(MullNet CompilerExtention) Evaluates weather or not each Element in the _Array is unique. In other words, False is Returned if two or more elements are the same.</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function AllElementsAreUnique(Of _TElement)(ByVal _Array As _TElement()) As Boolean
			Return (New HashSet(Of _TElement)(_Array)).Count = _Array.Length
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns the Maximun possible Index one could specify for a non-[dynamically-allocated], zero-indexed Array
		''' </summary>
		''' <param name="_Array"></param>
		''' <returns>Returns the Maximun possible Index one could specify for a non-[dynamically-allocated], zero-indexed Array</returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function MaxIndex%(ByRef _Array As Array)
			Return (_Array.Length - 1)
		End Function

		''' <summary>
		''' (MullNet CompilerExtention)
		''' </summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function PenultimateIndex%(ByRef _Array As Array)
			Return (_Array.Length - 2)
		End Function

		''' <summary>
		''' (MullNet CompilerExtention)
		''' </summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function ListValues(ByVal _List As List(Of UInt64)) As String
			Dim _ValuesString$ = "{"
			_List.ForEach(Sub(_Value As UInt64) _ValuesString &= (_Value & ", "))
			If Not _List.Count = 0 Then _ValuesString.RemoveLastTwoCharacters()
			Return _ValuesString & "}"
		End Function

	End Module

	''' <remarks>Needs [System.Web.dll]</remarks>
	Public Module WebObjectExtentions

		''' <summary>
		''' (MullNet CompilerExtention) Writes to, and then Ends the Response Stream
		''' </summary>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Sub WriteAndEnd(ByRef _ResponseObject As Web.HttpResponse, ByVal _String$)

			Try
				_ResponseObject.Write(_String) : _ResponseObject.SafelyEnd()
			Catch _Ex As Exception When True
				_ResponseObject.SafelyEnd()
			End Try

		End Sub

		''' <summary>
		''' (MullNet CompilerExtention) Clears, Writes to, and then Ends the Response Stream
		''' </summary>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Sub ClearAndWriteAndEnd(ByRef _ResponseObject As Web.HttpResponse, ByVal _String$)

			Try
				_ResponseObject.Clear() : _ResponseObject.Write(_String) : _ResponseObject.SafelyEnd()
			Catch _Ex As Exception When True
				_ResponseObject.SafelyEnd()
			End Try

		End Sub

		''' <summary>
		''' (MullNet CompilerExtention) Ends the Response Stream, without Throwing the "Thread was being aborted" Exception
		''' </summary>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Sub [SafelyEnd](ByRef _ResponseObject As Web.HttpResponse)

			Web.HttpContext.Current.Response.Flush() 'Sends all currently buffered output to the client.
			Web.HttpContext.Current.Response.SuppressContent = True	'Gets or sets a value indicating whether to send HTTP content to the client.
			Web.HttpContext.Current.ApplicationInstance.CompleteRequest() 'Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

			Return

		End Sub

		''' <summary>
		''' (MullNet CompilerExtention) Checks that the Specified QueryString is contained in the current Request Object.
		''' </summary>
		''' <param name="_RequestObject"></param>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function QueryStringIsSpecified(ByRef _RequestObject As Web.HttpRequest, ByVal _QueryStringKey$) As [Boolean]
			Try

				'Return _RequestObject.QueryString.AllKeys.Contains(_QueryStringKey)

				'A case-insensitive version:

				If (_QueryStringKey = Nothing) OrElse (_QueryStringKey Is Nothing) Then Throw New CompilerExtentionException("The specified _QueryStringKey Argument [Is Nothing].")

				_QueryStringKey = _QueryStringKey.[ToUpper]()

				For Each _Key$ In _RequestObject.QueryString.AllKeys
					If (_Key = Nothing) OrElse (_Key Is Nothing) Then Throw New CompilerExtentionException("At least one QueryString Key was [Nothing]. The QueryString may have ended with an [&] or may not have had an [=Value] after the Key.")
					If _Key.ToUpper() = _QueryStringKey Then Return True
				Next

				Return False

			Catch _Ex As Exception : Throw New CompilerExtentionException("@QueryStringIsSpecified (CompilerExtention): " & _Ex.Message, _Ex) : End Try
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Checks that the Specified PostData Item is contained in the current Request Object.
		''' </summary>
		''' <param name="_RequestObject"></param>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function PostDataItemIsSpecified(ByRef _RequestObject As Web.HttpRequest, ByVal _PostDataItemKey$) As [Boolean]

			'Return _RequestObject.Form.AllKeys.Contains(_PostDataItemKey$)

			'A case-insensitive version:

			_PostDataItemKey = _PostDataItemKey.[ToUpper]()

			For Each _Key$ In _RequestObject.Form.AllKeys
				If _Key.ToUpper() = _PostDataItemKey Then Return True
			Next

			Return False

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Checks that the Specified QueryString is contained in the current Request Object, and that it has a Value.
		''' </summary>
		''' <param name="_RequestObject"></param>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function QueryStringIsSpecifiedAndHasValue(ByRef _RequestObject As Web.HttpRequest, ByVal _QueryStringKey$) As [Boolean]

			Return (_RequestObject.QueryStringIsSpecified(_QueryStringKey) AndAlso (Not String.IsNullOrEmpty(_RequestObject.QueryString(_QueryStringKey))))

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Checks that the Specified PostData Item is contained in the current Request Object, and that it has a Value.
		''' </summary>
		''' <param name="_RequestObject"></param>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function PostDataItemIsSpecifiedAndHasValue(ByRef _RequestObject As Web.HttpRequest, ByVal _PostDataItemKey$) As [Boolean]

			Return (_RequestObject.PostDataItemIsSpecified(_PostDataItemKey) AndAlso (Not String.IsNullOrEmpty(_RequestObject.Form(_PostDataItemKey$))))

		End Function

	End Module

	Public Module XMLObjectExtentions

		''' <summary>(MullNet CompilerExtention) Returns a version of the _XElement wrapped in an XML Parent Node with name _OuterNodeName$</summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function WrapIn(ByVal _XElement As XElement, ByVal _OuterNodeName$) As XElement
			Try

				Dim _OuterXElement As New XElement(_OuterNodeName) : _OuterXElement.Add(_XElement)
				Return _OuterXElement

			Catch _Ex As Exception : Throw New CompilerExtentionException("@WrapIn (CompilerExtention): " & _Ex.Message, _Ex) : End Try
		End Function

	End Module

	''' <remarks>Needs [System.Drawing.dll]</remarks>
	Public Module ImageObjectExtentions

		''' <summary>
		''' (MullNet CompilerExtention) Modifies (and Returns) the _InputBitmap, resized to the Specified Width and Height
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function ResizeTo(ByRef _InputBitmap As Drawing.Bitmap, ByVal _TargetWidth%, ByVal _TargetHeight%) As Drawing.Bitmap
			Try

				Dim _OutputBitmapRectangle As Drawing.Rectangle = (New Drawing.Rectangle(0, 0, _TargetWidth, _TargetHeight))
				Dim _OutputBitmap As Drawing.Bitmap = (New Drawing.Bitmap(_TargetWidth, _TargetHeight))

				_OutputBitmap.SetResolution(_InputBitmap.HorizontalResolution, _InputBitmap.VerticalResolution)

				Using _Graphics As Drawing.Graphics = Drawing.Graphics.FromImage(_OutputBitmap)

					With _Graphics
						.CompositingMode = Drawing.Drawing2D.CompositingMode.SourceCopy
						.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
						.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
						.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
						.PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighQuality
					End With

					Using _WrapMode As Drawing.Imaging.ImageAttributes = (New Drawing.Imaging.ImageAttributes())

						_WrapMode.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
						_Graphics.DrawImage(_InputBitmap, _OutputBitmapRectangle, 0, 0, _InputBitmap.Width, _InputBitmap.Height, Drawing.GraphicsUnit.Pixel, _WrapMode)

					End Using

				End Using

				_InputBitmap = _OutputBitmap

				Return _OutputBitmap

			Catch _Ex As Exception When True : Throw New CompilerExtentionException("@ResizeTo: " & _Ex.Message, _Ex) : End Try
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Modifies (and Returns) the _InputBitmap, resizing both the Width and Height by the Specified _PercentageOfCurrentDimensions
		''' </summary>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function ResizeToPercentage(ByRef _InputBitmap As Drawing.Bitmap, ByVal _PercentageOfCurrentDimensions As UInt16) As Drawing.Bitmap

			If Not CLng(_PercentageOfCurrentDimensions).IsBetweenInclusive(1, 10000) Then Throw New CompilerExtentionException("The _PercentageOfCurrentDimensions for the Bitmap Resize was out of the permitted range (1 to 10000 Inclusive) at: " & _PercentageOfCurrentDimensions.ToString())

			Return _InputBitmap.ResizeTo(
			 _TargetWidth:=CInt(_InputBitmap.Width * (_PercentageOfCurrentDimensions / 100)),
			 _TargetHeight:=CInt(_InputBitmap.Height * (_PercentageOfCurrentDimensions / 100))
			)

		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Evaluates what the Dimensions of a Bitmap become, when it is 
		''' </summary>
		''' <returns></returns>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function GetConstrictedSize(ByRef _Bitmap As Drawing.Bitmap, ByVal _MaxDimension As UInt32) As Drawing.Size

			Try

				If _MaxDimension < 1 Then Throw New CompilerExtentionException("GetConstrictedSize(): The _MaxDimension cannot be less than 1")

				If (_Bitmap.Height > _MaxDimension) OrElse (_Bitmap.Width > _MaxDimension) Then

					'Multiply both of the Dimensions by [Max/LargestDimension]

					Return New Drawing.Size( _
					   CInt(_Bitmap.Width * (_MaxDimension / If(_Bitmap.Height > _Bitmap.Width, _Bitmap.Height, _Bitmap.Width))), _
					   CInt(_Bitmap.Height * (_MaxDimension / If(_Bitmap.Height > _Bitmap.Width, _Bitmap.Height, _Bitmap.Width)))
					)

				Else
					REM If we're here, then the _Bitmap is already within the specified DimensionalBounds
					Return _Bitmap.Size
				End If
			Catch _Ex As Exception When True
				Throw New CompilerExtentionException("The following Exception was thrown upon attempting to evaluate the Size of a Constricted Bitmap: " & _Ex.Message & ". The _MaxDimension was: " & _MaxDimension.ToString(), _Ex)
			End Try
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Makes sure that the Largest Dimension of the _Bitmap is no larger than the _MaxDimension, resizing the _Bitmap is needed.
		''' </summary>
		''' <returns></returns>
		''' <remarks>Modifies and Returns the _Bitmap</remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function MakeNoLargerThan(ByRef _Bitmap As Drawing.Bitmap, ByVal _MaxDimension As UInt32) As Drawing.Bitmap

			Try

				If _MaxDimension < 1 Then Throw New CompilerExtentionException("MakeNoLargerThan(): The _MaxDimension cannot be less than 1")

				If (_Bitmap.Height > _MaxDimension) OrElse (_Bitmap.Width > _MaxDimension) Then

					'Multiply both of the Dimensions by [Max/LargestDimension]

					Return _Bitmap.ResizeTo( _
					 CInt(_Bitmap.Width * (_MaxDimension / If(_Bitmap.Height > _Bitmap.Width, _Bitmap.Height, _Bitmap.Width))), _
					 CInt(_Bitmap.Height * (_MaxDimension / If(_Bitmap.Height > _Bitmap.Width, _Bitmap.Height, _Bitmap.Width)))
					)

				Else
					REM If we're here, then the _Bitmap is already within the specified DimensionalBounds
					Return _Bitmap
				End If
			Catch _Ex As Exception When True
				Throw New CompilerExtentionException("The following Exception was thrown upon attempting to Make a Bitmap no Larger than a MaxDimension: " & _Ex.Message & ". The _MaxDimension was: " & _MaxDimension.ToString(), _Ex)
			End Try
		End Function

		''' <summary>
		''' (MullNet CompilerExtention) Returns a copy of the _Bitmap, zooming in by the the _ZoomFactor
		''' </summary>
		''' <param name="_Bitmap"></param>
		''' <param name="_ZoomFactor">
		''' E.g. [0.8] would cause 10% of the width to be removed from both sides, and 10% of the height to be removed from both the top and bottom, going inwards towards the center of the image.
		''' In other words, a _ZoomFactor of 0.8 causes the Returned copy of the Image to be 80% of the height of the origional, and 80% of the width of the origional
		''' </param>
		''' <returns></returns>
		''' <remarks>
		''' A _ZoomFactor of [0.8] would cause 10% of the width to be removed from both sides, and 10% of the height to be removed from both the top and bottom (going inwards towards the center of the image).
		''' In other words, a _ZoomFactor of 0.8 causes the Returned copy of the Image to be 80% of the height of the origional, and 80% of the width of the origional
		''' 0.8 would therefore actually cause 64% of the pixels of the origional image to be kept, because 10*10 = 100, but 8*8 only = 64.
		''' </remarks>
		<Runtime.CompilerServices.Extension()>
		Public Function GetZoomedInCopy(ByRef _Bitmap As Drawing.Bitmap, Optional ByVal _ZoomFactor As Decimal = 0.8@) As Drawing.Bitmap

			Try

				REM The _ZoomFactor must be [lessthan or equal to 1], and [greater than 0]; We cannot Zoom Out
				If Not ((_ZoomFactor <= 1) AndAlso (_ZoomFactor > 0)) Then Throw (New CompilerExtentionException("The _ZoomFactor must be [lessthan or equal to 1], and [greater than 0]. It was, however: " & _ZoomFactor.ToString()))


				Dim _RectangleOfOrigionalToKeep As New Drawing.RectangleF( _
				 x:=((_Bitmap.Size.Width * (1 - _ZoomFactor)) / 2), _
				 y:=((_Bitmap.Size.Height * (1 - _ZoomFactor)) / 2), _
				 width:=(_Bitmap.Size.Width * _ZoomFactor), _
				 height:=(_Bitmap.Size.Height * _ZoomFactor) _
				)

				Return _Bitmap.Clone(_RectangleOfOrigionalToKeep, _Bitmap.PixelFormat)

			Catch _Ex As Exception When True
				Throw New CompilerExtentionException("The following Exception was Thrown upon attempting to Generate a Zoomed-In Copy of a System.Drawing.Bitmap: " & _Ex.Message, _Ex)
			End Try

		End Function

	End Module

	Public Module IOObjectExtentions

		''' <summary>
		''' (MullNet CompilerExtention) Gets the Size of the File in KB, to the nearest Integer
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		<Global.System.Runtime.CompilerServices.Extension()>
		Public Function SizeInIntegralKB(ByRef _File As IO.FileInfo) As UInt64

			Return CULng(_File.Length).BytesToIntegralKB()

		End Function

	End Module

	''' <summary>
	''' Non-CompilerExtention Useful Methods which need to be accessible without Namespace qualification
	''' Some of these can't be CompilerExtentions E.g. because they take in a ParamArray as the first Argument
	''' </summary>
	Public Module UsefulMethods

		''' <summary>
		''' (MullNet UsefulMethod) Returns a _TElement()() from the Array of _TElement Arrays
		''' </summary>
		Public Function Get2DArray(Of _TElement)(ByVal ParamArray _Array As _TElement()()) As _TElement()()
			Return _Array
		End Function

		''' <summary>
		''' (MullNet UsefulMethod)
		''' Ensures that for each opening component in the _Pairs, there is an instance of the corrosponding closing component.
		''' Also makes sure that the _Pairs components are opened in a ballenced fashion.
		''' E.g. if done with brackets, then "([])" would be valid, whereas "([)]" would not be (even though there are the same number of brackets and squares in the latter).
		''' 
		''' Example Call: ContainsWellBalencedPairs(Of String)({"Open", "Null", "Null", "Close", "Null", "Close"}, New Tuple(Of String, String)("Open", "Close"))
		''' <example>ContainsWellBalencedPairs(Of String)({"Open", "Null", "Null", "Close", "Null", "Close"}, New Tuple(Of String, String)("Open", "Close"))</example>
		''' 
		''' How this Function works:
		''' 'Declare a _TItems Stack
		''' For Each Item In _JustTheRelevantItems
		''' If the Item is an Opening Component (_Pair.Item1) then push it onto stack
		''' If the Item is a Closing Component (_Pair.Item2) then pop from stack and if the popped Item is the matching Opening Component then fine, but otherwise the Items are not balanced
		''' After complete traversal, if there is an Opening Component left in stack then the source is not balanced
		''' </summary>
		''' <typeparam name="_TItems">The DataType of each Item in the _AllItems, and of a component in the _Pairs</typeparam>
		''' <param name="_AllItems">A List of _TItems, not all of which have to be Items seen in the _Pairs. E.g. could be all source token even if you just want to validate the balencedness of brackets.</param>
		''' <param name="_Pairs">The Pairs which have to be balenced. Item1 = the Opening Component E.g. "("; Item2 = the Closing Component E.g. ")"</param>
		''' <returns>Weather or not _AllItems contained well-balenced Pairs</returns>
		''' <remarks>Implemented 24-08-2022 so that Brackets and Statements may make use of it</remarks>
		Public Function ContainsWellBalencedPairs(Of _TItems)(ByVal _AllItems As _TItems(), ByVal ParamArray _Pairs As Tuple(Of _TItems, _TItems)()) As Boolean
			Try

				'Declare a _TItems Stack
				'For Each Item In _JustTheRelevantItems
				'	If the Item is an Opening Component (_Pair.Item1) then push it onto stack
				'	If the Item is a Closing Component (_Pair.Item2) then pop from stack and if the popped Item is the matching Opening Component then fine, but otherwise the Items are not balanced
				'After complete traversal, if there is an Opening Component left in stack then the source is not balanced

				'Contains only Items which are also present in the _Pairs (E.g. only the Brackets [] {} <> out of all the Source Tokens)
				Dim _JustTheRelevantItems As _TItems() = (From _Item As _TItems In _AllItems Where (_Pairs.Any(Function(_Pair As Tuple(Of _TItems, _TItems)) (_Pair.Item1.Equals(_Item)) OrElse (_Pair.Item2.Equals(_Item)))) Select _Item).ToArray()
				Dim _ItemsStack As New Stack(Of _TItems)()
				Dim _NoOpeningComponents, _NoClosingComponents As UInt32

				For Each _Item As _TItems In _JustTheRelevantItems

					'Was advised to do this by VS 2010. Sure.
					Dim _LambdaCopyOf_Item As _TItems = _Item

					REM If we have an Opening Component, Push() it onto the Stack
					If _Pairs.Any(Function(_Pair As Tuple(Of _TItems, _TItems)) _Pair.Item1.Equals(_LambdaCopyOf_Item)) Then

						_ItemsStack.Push(_Item) : _NoOpeningComponents += 1UI

						REM If we have a Closing Component, find out if _ItemsStack.Pop() produces the corrosponding Opening Component
					ElseIf _Pairs.Any(Function(_Pair As Tuple(Of _TItems, _TItems)) _Pair.Item2.Equals(_LambdaCopyOf_Item)) Then

						'Get the Pair which contains the corrosponding Opening Component for out Closing Component
						Dim _Pair_WhereforWeHaveClosingComponent As Tuple(Of _TItems, _TItems) = _
						 _Pairs.First(Function(_Pair As Tuple(Of _TItems, _TItems)) _Pair.Item2.Equals(_LambdaCopyOf_Item))

						'Now see if the Pop() produces the same Opening Component as we have in our Pair
						'(Additionally, if the Stack is empty, then the Items aren't well-balenced, because we just hit a Closing Component which didn't follow a previous corrosponding Opening Component)
						If (_ItemsStack.Count = 0) OrElse (Not _ItemsStack.Pop().Equals(_Pair_WhereforWeHaveClosingComponent.Item1)) Then Return False
						_NoClosingComponents += 1UI

					Else : Throw New CompilerExtentionException("An Item was not recognised as either an Opening or Closing Component. The _Item's ToString() is: " & _Item.ToString())
					End If

				Next

				REM Now determine if there is anything left on the Stack. The only things on there could ever be Opening Components, because that's all we ever Push()
				If Not (_ItemsStack.Count = 0) Then Return False

				REM If we're here, then all Opening and Closing Components must have been balenced, because we haven't yet Return'd
				'Log(String.Format("Determined that {0} relevant Item(s) out of {1} total Item(s) were well-balenced with {2} Opening Component(s) and {3} Closing Component(s) from {4} Pair(s)", _JustTheRelevantItems.Length.ToString(), _AllItems.Length.ToString(), _NoOpeningComponents.ToString(), _NoClosingComponents.ToString(), _Pairs.Length.ToString()), LogEvent.DSEventSeverity.Verbose)
				Return True

			Catch _Ex As Exception : Throw New CompilerExtentionException("@IsStackBalenced(Of " & GetType(_TItems).Name & "): " & _Ex.Message, _Ex) : End Try
		End Function

	End Module

	Public Class CompilerExtentionException : Inherits System.Exception

		Public Sub New(ByVal _Message$)
			MyBase.New("[CompilerExtentionException] " & _Message)
		End Sub

		Public Sub New(ByVal _Message$, ByVal _InnerException As System.Exception)
			MyBase.New("[CompilerExtentionException] " & _Message, _InnerException)
		End Sub

	End Class

End Namespace