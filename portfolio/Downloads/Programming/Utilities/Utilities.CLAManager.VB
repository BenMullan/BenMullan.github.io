'BM Utilities CLAManager
'---------------------------------------
'(Manages Command-Line Arguments)

'Features:
'	- Handles [/Key:Value] and [/Key] Pairs with [Action(Of String)]s
'	- Automatically recognises the /? switch and displays a CLA Help Dictionary

'Required Syntax: (Example)
'	Program.exe /Key1:Value /Key2:"Value with Spaces" /Key3
'	In other words, the KVP must be in one OS-defined (space-delimited) CLA

'Example Instanciation:
'	Dim _CLAManager As New DocScript.Utilities.CLAManager(
'		_CLAs,
'		"DocScript Command-Line Interpreter. Interprets DocScript Source Files.",
'		{"DSCLI.EXE /RunSourceString:""Function <Void> Main ();Output(`Hello, World!`);EndFunction""",
'		"DSCLI.EXE /RunSourceFile:""X:\Programming\DocScript\HelloWorld.DS"""},
'		AddressOf Console.WriteLine,
'		New Utilities.CLAManager.CLADatum("RunSourceFile", "Interprets the specified DocScript Source File", False, True, Sub(_Value$) Beep()),
'		New Utilities.CLAManager.CLADatum("RunSourceString", "Interprets the specified DocScript Source String. Use ; for NewLine and ` for StringLiteralStartEndChar.", False, True, Sub(_Value$) Beep()),
'		New Utilities.CLAManager.CLADatum("ShowLog", "Writes Events from the DocScript Log to the Console Output Stream during Interpretation", False, False, Sub(_Value$) EntryPoint.ShowLog = True),
'		New Utilities.CLAManager.CLADatum("ProcessDebugEvents", "Processes and shows Debugging Messages in the Log (if the Log is shown)", False, False, Sub(_Value$) DocScript.Logging.LogUtilities.ProcessDebugEvents = True)
'	)

Namespace Utilities

	''' <summary>
	''' Manages a set of CLAData to act on any specified Command-Line Arguments, and to display a CLA Help Dictionary if the argument /? is specified.
	''' </summary>
	''' <remarks></remarks>
	Partial Public Class CLAManager

		Public Const CLAKeyStartChar As Char = "/"c	'E.g.	/ShowLog
		Public Const CLAValueStartChar As Char = ":"c
		Public Const HelpCLA$ = CLAKeyStartChar & "?"c

		Public ReadOnly ProvidedCLAs As ObjectModel.ReadOnlyCollection(Of [String])
		Public ReadOnly OutputDelegate As Action(Of [String])
		Public ReadOnly CLAData As ObjectModel.ReadOnlyCollection(Of CLADatum)
		Public ReadOnly CLAHelpDictionary_Description As [String]
		Public ReadOnly CLAHelpDictionary_Examples As [String]()

		''' <summary>
		''' Constructs a Command-Line Argument Manager. All managment of the CLAs occours as soon as the object is constructed. No additional call is nessecary.
		''' 
		''' Exmaple Call:
		''' Dim _CLAManager As New DocScript.Utilities.CLAManager(
		'''    _CLAs,
		'''    "DocScript Command-Line Interpreter. Interprets DocScript Source Files.",
		'''    {"DSCLI.EXE /RunSourceString:""Function &lt;Void&gt; Main ();Output(`Hello, World!`);EndFunction""",
		'''    "DSCLI.EXE /RunSourceFile:""X:\Programming\DocScript\HelloWorld.DS"""},
		'''    AddressOf Console.WriteLine,
		'''    New Utilities.CLAManager.CLADatum("RunSourceFile", "Interprets the specified DocScript Source File", False, True, Sub(_Value$) Beep()),
		'''    New Utilities.CLAManager.CLADatum("RunSourceString", "Interprets the specified DocScript Source String. Use ; for NewLine and ` for StringLiteralStartEndChar.", False, True, Sub(_Value$) Beep()),
		'''    New Utilities.CLAManager.CLADatum("ShowLog", "Writes Events from the DocScript Log to the Console Output Stream during Interpretation", False, False, Sub(_Value$) EntryPoint.ShowLog = True),
		'''    New Utilities.CLAManager.CLADatum("ProcessDebugEvents", "Processes and shows Debugging Messages in the Log (if the Log is shown)", False, False, Sub(_Value$) DocScript.Logging.LogUtilities.ProcessDebugEvents = True)
		''' )
		''' 
		''' If the Help Dictionary is displayed, then no other CLA Actions will be dealt with or run
		''' </summary>
		''' <param name="_CLAs">The Command-Line Arguments to deal with E.g. My.Application.CommandLineArgs.ToArray() or an "args" Parameter</param>
		''' <param name="_CLAData">A collection of CLADatum Instances (#KnowYourLatin)</param>
		''' <param name="_CLAHelpDictionary_Description">The Text displayed at the start of the CLA Help Dictionary, when it is displayed via the presence of the /? CLA</param>
		''' <param name="_CLAHelpDictionary_Examples">Example Calls to the Binary executable with CLAs. E.g. DSCLI.EXE /RunSourceFile:"X:\Programming\DocScript\HelloWorld.DS"</param>
		''' <param name="_OutputDelegate">Used for E.g. outputting the CLA Help Dictionary if the /? CLA is specified</param>
		''' <remarks></remarks>
		Public Sub New(ByVal _CLAs As String(), ByVal _CLAHelpDictionary_Description$, ByVal _CLAHelpDictionary_Examples$(), ByVal _OutputDelegate As Action(Of String), ByVal ParamArray _CLAData As CLADatum())
			Try

				REM Ensure all Keys are Unique
				If Not _CLAData.Select(Of String)(Function(_CLADatum As CLADatum) _CLADatum.Key).ToArray().AllElementsAreUnique() Then Throw New Exception("The CLAData Keys were not all Unique.")

				Me.OutputDelegate = _OutputDelegate
				Me.ProvidedCLAs = New ObjectModel.ReadOnlyCollection(Of String)(_CLAs.ToList())
				Me.CLAData = New ObjectModel.ReadOnlyCollection(Of CLADatum)(_CLAData.ToList())
				Me.CLAHelpDictionary_Description = _CLAHelpDictionary_Description : Me.CLAHelpDictionary_Examples = _CLAHelpDictionary_Examples

				If Me.ProvidedCLAs.Contains(HelpCLA) Then
					Me.DisplayHelpDictionary()
				Else
					Me.ExecuteActionsForSpecifiedCLAs()
				End If

			Catch _Ex As Exception When True : Throw New Exception("Whilst constructing the Command-Line Argument Manager: " & _Ex.Message, _Ex) : End Try
		End Sub

		Public Shared Function CLAsContainsKey(ByVal _CLAs$(), ByVal _Key$) As Boolean

			'											/Key											/Key: {...}
			Return _CLAs.Any(Function(_CLA$) (_CLA = CLAKeyStartChar & _Key) OrElse _CLA.StartsWith(CLAKeyStartChar & _Key & CLAValueStartChar))

		End Function

		Protected Sub ExecuteActionsForSpecifiedCLAs()

			REM Ensure the Mandatory Keys have been specified, and execute the Actions on derivable <Value>s
			For Each _CLADatum As CLADatum In Me.CLAData

				If _CLADatum.MustBeSpecified Then _
				 If Not CLAsContainsKey(Me.ProvidedCLAs.ToArray(), _CLADatum.Key) _
				  Then Throw New CLANotSpecifiedException("The mandatory Command-Line Argument with Key """ & _CLADatum.Key & """ was not specified. Command-Line Arguments must be provided in the syntax  /Key:Value  or  /Key:""Value""  or  /Key .")

				'Here, we know that *if* the _CLADatum was mandatory, then it has been specified
				'If it hasn't been specified, then we don't need to do anything about it
				If Not CLAsContainsKey(Me.ProvidedCLAs.ToArray(), _CLADatum.Key) Then Continue For

				'If we're here, then the CLA Has been specified, so ensure it's value is present if it needs one
				Dim _CorrospondingValue$ = Nothing

				If _CLADatum.MustHaveValue Then

					Dim _LambdaCopyOf_CLADatum As CLADatum = _CLADatum

					Dim _CLAWithMatchingKey_Resolver As Func(Of String, Boolean) = _
					   Function(_CLA$) _CLA.StartsWith(CLAKeyStartChar & _LambdaCopyOf_CLADatum.Key & CLAValueStartChar) AndAlso (_CLA.Length > (CLAKeyStartChar & _LambdaCopyOf_CLADatum.Key & CLAValueStartChar).Length)

					'Ensure that there is a Value
					If Not (Me.ProvidedCLAs.Where(_CLAWithMatchingKey_Resolver).Count() = 1) Then _
					 Throw New CLAValueNotSpecifiedException("The Command-Line Argument with Key """ & _CLADatum.Key & """ declared that a corrosponding Value was mandatory, but no recognisable value was provided in the Syntax /Key:Value or /Key:""Value"".")

					'Extract the :Value or :"Value"
					_CorrospondingValue = New String(Me.ProvidedCLAs.First(_CLAWithMatchingKey_Resolver).ToCharArray().Skip(Me.ProvidedCLAs.First(_CLAWithMatchingKey_Resolver).ToCharArray().ToList().IndexOf(CLAValueStartChar) + 1).ToArray())

				End If

				REM Now run the Action, using the derived <Value>
				_CLADatum.ActionIfSpecified.Invoke(_CorrospondingValue)

			Next

		End Sub

		''' <summary>Writes the CLA Help Dictionary to the OutputDelegate if the /? CLA is specified</summary>
		Protected Sub DisplayHelpDictionary()

			REM CLA Help Dictionary Format:

			'Description:
			'----------------------------------------
			'{Description}
			'
			'Examples:
			'----------------------------------------
			'{Examples}
			'
			'Argument Usage:
			'----------------------------------------
			'/RunFile:<Value>		The File to Execute (*.DS)
			'/Debug					Shows Debugging Logging Messages
			'/ShowLog				Shows the Log during Interpretation				

			'Use Colouring if Console.WriteLine is the OutputDelegate
			'If the OutputDelegate isn't Console.WriteLine, then this won't hurt anything...
			'Dim _PreviousConsoleColour As ConsoleColor = Console.ForegroundColor
			'Console.ForegroundColor = ConsoleColor.Cyan

			Dim _CLAHelpDictionaryText$ = _
			  "Description:" & vbCrLf & _
			  "----------------------------------------" & vbCrLf & _
			  Me.CLAHelpDictionary_Description & vbCrLf & vbCrLf & _
			  "Examples:" & vbCrLf & _
			  "----------------------------------------" & vbCrLf

			For Each _Example$ In Me.CLAHelpDictionary_Examples
				_CLAHelpDictionaryText &= _Example & vbCrLf
			Next

			_CLAHelpDictionaryText &= vbCrLf & _
			 "Argument Usage:" & vbCrLf & _
			 "----------------------------------------" & vbCrLf

			For Each _CLADatum As CLADatum In Me.CLAData

				Dim _KeyAndValuePart$ = (CLAKeyStartChar & _CLADatum.Key) & If(_CLADatum.MustHaveValue, CLAValueStartChar & "<Value>", String.Empty)
				Const _KeyAndValuePart_TargetLength% = 30I

				REM The Descriptions start 30 Chars in.
				_CLAHelpDictionaryText &= _KeyAndValuePart.PadRight(_KeyAndValuePart_TargetLength, " "c) & _CLADatum.Description & vbCrLf

			Next

			Me.OutputDelegate.Invoke(_CLAHelpDictionaryText)

			'Console.ForegroundColor = _PreviousConsoleColour

			Return

		End Sub

	End Class

	REM This can go in a seperate file, if need be.
	Partial Public Class CLAManager

		''' <summary>
		''' Represents a configuration for a single CLA key, and what to do if it's specified.
		''' (Specified in the Syntax /Key or /Key:Value or /Key:"Value")
		''' </summary>
		''' <remarks></remarks>
		Public Class CLADatum

			Public ReadOnly Key As [String]
			Public ReadOnly Description As [String]
			Public ReadOnly MustBeSpecified As [Boolean]
			Public ReadOnly MustHaveValue As [Boolean]
			Public ReadOnly ActionIfSpecified As Action(Of String)

			''' <summary>
			''' Constructs a new Object to represent the desired configuration for a single CLA Key (Specified in the Syntax /Key or /Key:Value or /Key:"Value")
			''' </summary>
			''' <param name="_Key">The CLA Key E.g. "ShowLog". The forward slash / is not to be included in this string</param>
			''' <param name="_Description">The Description used in the CLA Help Dictionary ( /? )</param>
			''' <param name="_MustBeSpecified">Indicates that the Key must be specified in the Command-Line Arguments provided to the CLAManager, otherwise an Exception is Thrown.</param>
			''' <param name="_MustHaveValue">Indicates that if the Key is specified, then a corrosponding Value must also be specified, otherwise an Exception is Thrown.</param>
			''' <param name="_ActionIfSpecified">The Action to run if the Key is specified. The String Argument is the Value corrosponding to the Key, which could be [Nothing] (null) {nullptr} &lt;undefined&gt; if there is no Value corrosponding to the Key</param>
			''' <remarks></remarks>
			Public Sub New( _
			 ByVal _Key$,
			 ByVal _Description$,
			 ByVal _MustBeSpecified As Boolean,
			 ByVal _MustHaveValue As Boolean,
			 ByVal _ActionIfSpecified As Action(Of String) _
			)

				If _Key.Contains(CLAManager.CLAKeyStartChar) OrElse _Key.Contains(CLAManager.CLAValueStartChar) Then Throw New System.Exception("A Command-Line Argument Key in a CLADatum cannot contain the CLAKeyStartChar " & CLAManager.CLAKeyStartChar & " or the CLAValueStartChar " & CLAManager.CLAValueStartChar)

				Me.Key = _Key
				Me.Description = _Description
				Me.MustBeSpecified = _MustBeSpecified
				Me.MustHaveValue = _MustHaveValue
				Me.ActionIfSpecified = _ActionIfSpecified

			End Sub

		End Class

	End Class

	''' <summary>Thrown when a Command-Line Argument is mandatory, but not specified. Does NOT Inherit DSException.</summary>
	Public Class CLANotSpecifiedException : Inherits System.Exception

		Public Sub New(ByVal _Message$)
			MyBase.New("[DSCLANotSpecifiedException] " & _Message)
		End Sub

	End Class

	''' <summary>Thrown when a Command-Line Argument is specified, but without a mandatory associated Value in the form /Key:Value or /Key:"Value". Does NOT Inherit DSException.</summary>
	Public Class CLAValueNotSpecifiedException : Inherits System.Exception

		Public Sub New(ByVal _Message$)
			MyBase.New("[DSCLAValueNotSpecifiedException] " & _Message)
		End Sub

	End Class

End Namespace