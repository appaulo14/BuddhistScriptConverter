Public MustInherit Class NonUnicodePaliScript
    Inherits PaliScript

    ''' <summary>
    ''' Halant Offset property
    ''' </summary>
    ''' <returns>The integer value needed to get from the consonant 
    ''' symbol to the vowel-suppressed version (halant) by adding 
    ''' Unicode code points</returns>
    Protected MustOverride ReadOnly Property HalantOffset() As Integer
    ''' <summary>
    ''' Anusvara offset property.
    ''' </summary>
    ''' <returns>The integer value needed to get from a symbol to 
    ''' its anusvared version when adding Unicode code points</returns>
    Protected MustOverride ReadOnly Property AnusvaraOffset() As Integer
    ''' <summary>
    ''' Matra offset property
    ''' </summary>
    ''' <returns>The integer value needed to get from a consonant to 
    ''' the various dependent vowel forms (matras). This value gets 
    ''' added repeated to go from one matras to the next</returns>
    Protected MustOverride ReadOnly Property MatraOffset() As Integer

    Protected Overrides Function ApplyAnusvara(ByVal inSymbol As String) As String
        Return ChrWH(Hex(AscW(inSymbol) + AnusvaraOffset))
    End Function

    Protected Overrides Function ApplyVowelSupression(ByVal inConsonant As String) As String
        Return ChrWH(Hex(AscW(inConsonant) + HalantOffset))
    End Function

    Public Overrides ReadOnly Property MySymbolOrder() As PaliScript.SymbolOrder
        Get
            Return SymbolOrder.IndicSource
        End Get
    End Property

    Protected Overrides Function PopulateMatras(ByVal inConsonants As System.Collections.Generic.List(Of String)) As System.Collections.Generic.List(Of String)
        Dim matras As New List(Of String)
        For Each inConsonant As String In inConsonants
            For i As Integer = 1 To 7
                matras.Add(ChrWH(Hex(AscW(inConsonant) + MatraOffset * i)))
            Next
        Next
        Return matras
    End Function
End Class
