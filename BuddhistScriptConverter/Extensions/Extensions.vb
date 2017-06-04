Imports System.Runtime.CompilerServices

Public Module Extensions
    ''' <summary>
    ''' Coverts the value of this character instance to its UTF-8 Hexadecimal representation
    ''' </summary>
    ''' <returns>A hexadecimal representation of the character Unicode point as a String</returns>
    <Extension()> _
    Function ToHex(ByVal myChar As Char) As String
        Return Hex(AscW(myChar))
    End Function

    ''' <summary>
    ''' Convert all the values of an IEnumberable(Of Char) into their UTF-8 hexadecimal equivalents
    ''' </summary>
    ''' <param name="myChars"></param>
    ''' <returns>An IEnumerable(Of Char) containing the input values converted into hexadecimal</returns>
    <Extension()> _
    Function ToHex(ByVal myChars As IEnumerable(Of Char)) As IEnumerable(Of String)
        Dim myHexPoints As New List(Of String)
        For Each myChar As Char In myChars
            myHexPoints.Add(myChar.ToHex)
        Next
        Return myHexPoints.AsEnumerable()
    End Function

    ''' <summary>
    ''' Takes a hexadecimal value and returns the equivalent UTF-8 character.
    ''' </summary>
    ''' <returns>the equivalent UTF-8 character for the inputted hex value</returns>
    Function ChrWH(ByVal inHex As String) As Char
        Return ChrW(Integer.Parse(inHex, System.Globalization.NumberStyles.AllowHexSpecifier))
    End Function
End Module
