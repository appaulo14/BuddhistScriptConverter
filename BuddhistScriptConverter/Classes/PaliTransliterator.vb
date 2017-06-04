Imports System.Reflection

Public Class PaliTransliterator
    ''' <summary>
    ''' A list of all scripts that will be used for transliteration
    ''' </summary>
    Public PaliScripts As New List(Of PaliScript)
    ''' <summary>
    ''' The script that all of the symbols will be transliterated to
    ''' </summary>
    ''' <remarks></remarks>
    Public DestinationPaliScript As PaliScript

    'Get the assembly that contains this code
    Dim asm As Assembly = Assembly.GetExecutingAssembly()

    ''' <summary>
    ''' Use introspection to get a full list of available scripts
    ''' </summary>
    Public Sub New()
        Dim allTypes As Type() = asm.GetTypes()
        For Each loopType As Type In allTypes
            'Only scan classes are descendent of PaliScript and not abstract
            If loopType.IsSubclassOf(GetType(PaliScript)) And Not loopType.IsAbstract() Then
                'Create any instance of the class
                Dim myPaliScript As PaliScript = _
                    CType(asm.CreateInstance(loopType.FullName, True, _
                    BindingFlags.CreateInstance, Nothing, Nothing, Nothing, Nothing), PaliScript)
                'Add it to the list of available PaliScripts
                PaliScripts.Add(myPaliScript)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Take a string and converts in to the script specified in the 
    ''' DestinationPaliScript property.
    ''' </summary>
    ''' <param name="inString">The string to transliterate</param>
    ''' <returns>The inputted string, transliterated to the 
    ''' DestinationPaliScript property</returns>
    Public Function Transliterate(ByVal inString As String) As String
        'Do some validation
        If (DestinationPaliScript Is Nothing) Then
            Throw New NullReferenceException("DestinationPaliScript property is not set (and should be)")
        End If
        'Convert to lowercase and remove Zero Width Joiners (ZWJs)
        inString = inString.ToLower.Replace(ChrW(8205), "")
        For Each srcScript As PaliScript In PaliScripts
            'Don't use the destination script in the conversion
            If (srcScript.Name = DestinationPaliScript.Name) Then
                Continue For
            End If
            'Replace any characters if found
            Dim srcSymbolOrder As PaliScript.SymbolOrder = srcScript.MySymbolOrder
            Dim srcScriptSymbols As List(Of String) = srcScript.Symbols(srcSymbolOrder)
            Dim destScriptSymbols As List(Of String) = DestinationPaliScript.Symbols(srcSymbolOrder)
            For i As Integer = 0 To srcScriptSymbols.Count - 1
                inString = inString.Replace(srcScriptSymbols(i), destScriptSymbols(i))
            Next
        Next
        Return inString
    End Function
End Class
