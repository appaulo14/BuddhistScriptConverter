Public MustInherit Class UnicodePaliScript
    Inherits PaliScript

    ''' <summary>
    ''' Returns the vowel suppression (halant symbols for this script)
    ''' </summary>
    ''' <returns>the vowel suppression (halant symbols for this script)</returns>
    Protected MustOverride ReadOnly Property Halant() As String

    Protected Overrides Function ApplyAnusvara(ByVal inSymbol As String) As String
        Return inSymbol & Me.Anusvara
    End Function

    Protected Overrides Function ApplyVowelSupression(ByVal inConsonant As String) As String
        Return inConsonant & Halant
    End Function

    Protected Overrides Function PopulateMatras(ByVal inConsonants As System.Collections.Generic.List(Of String)) As System.Collections.Generic.List(Of String)
        Dim matras As New List(Of String)
        For Each inConsonant As String In inConsonants
            For Each vowelSign As String In VowelSigns
                matras.Add(inConsonant & vowelSign)
            Next
        Next
        Return matras
    End Function

End Class
