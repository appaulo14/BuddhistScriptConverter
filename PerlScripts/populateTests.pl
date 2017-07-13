#!/usr/bin/perl
use warnings;
use strict;

#Define all the Pali consonants and vowels
my @consonants = qw(k kh g gh ɲ c ch  j  jh  ñ ṭ  ṭh ḍ ḍh  ṇ t  th d dh  n p ph b bh m y r l v s h ḷ ṃ);
my @vowels = qw(a ā i ī u ū e o);
#Open a file for writing
open my $file, ">", "myfile.txt";
#Loop through all the consonants, writing a test subroutine code to the file
foreach my $consonantIndex (0 .. scalar(@consonants) -1){
    my $consonant = $consonants[$consonantIndex];
    #Write the subroutine header information to the file
	print $file qq{'''<summary>;	
    '''Test that the $consonant vowel forms are properly defined
    '''</summary>
    <TestMethod()> _
    <Description("Test that $consonant vowel forms are properly defined")> _
    Public Sub ${consonant}Forms_ProperDefinition_EqualityExpected()\n};
    #Write the vowel assert statements to the file
    foreach my $vowelIndex (0 .. scalar(@vowels) -1){
        my $vowel = $vowels[$vowelIndex];
        print $file qq{Assert.AreEqual(Of String)(dependentVowelForms("$consonant$vowel"), "$consonant$vowel")\n};
    }
    #Write "End Sub" to finish the sub
    print $file "End Sub\n\n";
}