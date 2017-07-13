#!/usr/bin/perl
use warnings;
use strict;
use XML::Simple;
use Data::Dumper;
use charnames ':full';
use utf8;

#Declare some global variables
my $xmlPathName = "../BuddhistScriptConverter/XML";
my @vowelSigns = (
    "\N{DEVANAGARI VOWEL SIGN AA}", "\N{DEVANAGARI VOWEL SIGN I}",
    "\N{DEVANAGARI VOWEL SIGN II}", "\N{DEVANAGARI VOWEL SIGN U}", 
    "\N{DEVANAGARI VOWEL SIGN UU}", "\N{DEVANAGARI VOWEL SIGN E}", 
    "\N{DEVANAGARI VOWEL SIGN O}"
);
my @romanVowels = qw(a ā i ī u ū e o);
#Create xml parser object
my $xmlReader = XML::Simple->new();
#read the xml file
my $devaData = $xmlReader->XMLin("$xmlPathName/DevanagariPaliScript.xml");
my $romanData = $xmlReader->XMLin("$xmlPathName/RomanPaliScript.xml");
#print Dumper($data);
my $newData;
#Define new a new data structure to output
##Define the diferrent sections
my @irregulars = ("\N{DEVANAGARI SIGN ANUSVARA}");
my @vowels = XML_To_Array($devaData->{'vowels'}->{'element'});
###Put the matras ending a in a separate place with the non-a endings
my @refs = Matra_XML_To_Array($devaData->{'dependentVowelForms'}->{'element'});
my @matras = @{$refs[0]};
my @consonants = @{$refs[1]};
my @conjunctConsonants = @{GenerateConjunctConsonants(\@consonants)};
my @conjunctMatras = @{GenerateConjunctMatras(\@conjunctConsonants)};
my @numbers = XML_To_Array($devaData->{'numbers'}->{'element'});
##Put them all together into one big array
my %outputArray = (
    'irregulars'            => {'symbol' => \@irregulars},
    'conjunctMatras'        => {'symbol' => \@conjunctMatras},
    'conjunctConsonants'    => {'symbol' => \@conjunctConsonants},
    'consonants'            => {'symbol' => \@consonants},
    'vowels'                => {'symbol' => \@vowels},
    'numbers'               => {'symbol' => \@numbers},
);
my $xmlWriter = XML::Simple->new(NoAttr=>1, RootName=>'script');
my $xmlOutput = $xmlWriter->XMLout(\%outputArray);
$xmlOutput =~ s/anon>/symbol>/g;
open my $file, ">", "zhorse.xml" or die;
print $file "$xmlOutput";
close $file;
#my @vowels = [
#    {'roman' => 'a', 'devanagari' => 'अ'},
#];
my $x = 1;
# create array
#@arr = [ 
#        {'country'=>'england', 'capital'=>'london'},
#        {'country'=>'norway', 'capital'=>'oslo'},
#        {'country'=>'india', 'capital'=>'new delhi'} ];

sub XML_To_Array{
    my @array;
    #loop through the XML structure, pulling out the data and putting it in 
    #the new, correct format
    foreach my $devaSymbol (@{$_[0]}){
        my $romanForm = $devaSymbol->{'romanForm'};
        my $devaForm = $devaSymbol->{'content'};
        push @array, {'roman' => $romanForm, 'devanagari' => $devaForm};
    }
    #return the array
    return @array;
}

sub Matra_XML_To_Array{
    my @matraArray;
    my @consonantArray;
    #loop through the XML structure, pulling out the data and putting it in 
    #the new, correct format
    foreach my $devaSymbol (@{$_[0]}){
        my $romanForm = $devaSymbol->{'romanForm'};
        my $devaForm = $devaSymbol->{'content'};
        if ($romanForm =~ /a\z/){
            push @consonantArray, {'roman' => $romanForm, 'devanagari' => $devaForm};
        }
        else{
            push @matraArray, {'roman' => $romanForm, 'devanagari' => $devaForm};
        } 
    }
    #return the array
    return (\@matraArray,\@consonantArray);
}

sub GenerateConjunctConsonants{
    #Declare variables/get parameters
    my @consonantPairs = @{$_[0]};
    my @conjunctConsonantPairs;
    #Create the conjunct consonants by merging every consonant with every other one
    foreach my $consonantPair1 (@consonantPairs){
        foreach my $consonantPair2 (@consonantPairs){
            #Extract the necessary information from the consonant pairs
            my $romanConsonant1 = $consonantPair1->{'roman'};
            ##Remove the trailing 'a' sound from the roman version
            $romanConsonant1 =~s/a\z//;
            my $devaConsonant1  = $consonantPair1->{'devanagari'};
            my $romanConsonant2 = $consonantPair2->{'roman'};
            my $devaConsonant2  = $consonantPair2->{'devanagari'};
            my $conjunctConsonantPair = {
                'devanagari' => "$devaConsonant1\N{DEVANAGARI SIGN VIRAMA}$devaConsonant2",
                'roman'      => "$romanConsonant1$romanConsonant2",
            };
            push @conjunctConsonantPairs, $conjunctConsonantPair;
        }
    }
    #return the completed list
    return \@conjunctConsonantPairs;
}

sub GenerateConjunctMatras{
    #Declare variables/get parameters
    my @conjunctConsonantPairs = @{$_[0]};
    my @conjunctMatraPairs;
    #Create the conjunct matras by adding each of the 7 vowel signs onto
    #all of the conjunct consonants
    foreach my $conjunctConsonantPair(@conjunctConsonantPairs){
        foreach my $vowelIndex (0 .. @vowelSigns -1){
            #Extract the necessary information from the consonant pairs
            my $romanConjunctConsonant = $conjunctConsonantPair->{'roman'};
            ##Remove the trailing 'a' sound from the roman version
            $romanConjunctConsonant =~s/a\z//;
            my $devaConjunctConsonant  = $conjunctConsonantPair->{'devanagari'};
            my $conjunctMatraPair = {
                'devanagari' => "$devaConjunctConsonant$vowelSigns[$vowelIndex]",
                'roman'      => "$romanConjunctConsonant$romanVowels[$vowelIndex+1]",
            };
            push @conjunctMatraPairs, $conjunctMatraPair;
        }
    }
    #return the completed list
    return \@conjunctMatraPairs;
}