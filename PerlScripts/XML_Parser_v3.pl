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
#Define new a new data structure
##Define the diferrent sections
my @irregulars;
push @irregulars, {'roman' => "ṃ", 'devanagari' => "\N{DEVANAGARI SIGN ANUSVARA}"};
my @vowels = XML_To_Array($devaData->{'vowels'}->{'element'});
###Put the matras ending a in a separate place with the non-a endings
my @refs = Matra_XML_To_Array($devaData->{'dependentVowelForms'}->{'element'});
my @monoMatras = @{$refs[0]};
my @diMatras   = @{$refs[1]};
my @monoConsonants = @{$refs[2]};
my @diConsonants = @{$refs[3]};
my @monoDeadConsonants = @{GetDeadConsonants(\@monoConsonants)};
my @diDeadConsonants   = @{GetDeadConsonants(\@diConsonants)};
my @numbers = XML_To_Array($devaData->{'numbers'}->{'element'});
#Sort the consonants and dead consonants
#@consonants = reverse sort {$a->{'roman'} cmp $b->{'roman'}} @consonants;
#@deadConsonants = reverse sort {$a->{'roman'} cmp $b->{'roman'}} @deadConsonants;
#Put them all together into one big array
my %outputArray = (
    'irregulars'            => {'symbol' => \@irregulars},
    'diMatras'              => {'symbol' => \@diMatras},
    'monoMatras'            => {'symbol' => \@monoMatras},
    'diConsonants'          => {'symbol' => \@diConsonants},
    'monoConsonants'        => {'symbol' => \@monoConsonants},
    'diDeadConsonants'      => {'symbol' => \@diDeadConsonants},
    'monoDeadConsonants'    => {'symbol' => \@monoDeadConsonants},
    'vowels'                => {'symbol' => \@vowels},
    'numbers'               => {'symbol' => \@numbers},
);
my $xmlWriter = XML::Simple->new(NoAttr=>1, RootName=>'script');
my $xmlOutput = $xmlWriter->XMLout(\%outputArray);
$xmlOutput =~ s/anon>/symbol>/g;
open my $file, ">", "ScriptDefinitions3.xml" or die;
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
    my @monoMatras;
    my @diMatras;
    my @monoConsonants;
    my @diConsonants;
    #loop through the XML structure, pulling out the data and putting it in 
    #the new, correct format
    foreach my $devaSymbol (@{$_[0]}){
        my $romanForm = $devaSymbol->{'romanForm'};
        my $devaForm = $devaSymbol->{'content'};
        if ($romanForm =~ /a\z/){
            #Place the symbol based on whether it's consonant component
            #contains one or two characters
            if (length($romanForm) == 3){
                push @diConsonants, {'roman' => $romanForm, 'devanagari' => $devaForm};
            }
            else{  
                push @monoConsonants, {'roman' => $romanForm, 'devanagari' => $devaForm};
            }
        }
        else{
            #Place the symbol based on whether it's consonant component
            #contains one or two characters
            if (length($romanForm) == 3){
                push @diMatras, {'roman' => $romanForm, 'devanagari' => $devaForm};
            }
            else{  
                push @monoMatras, {'roman' => $romanForm, 'devanagari' => $devaForm};
            }
        } 
    }
    #return the array
    return (\@monoMatras,\@diMatras,\@monoConsonants,\@diConsonants);
}

sub GetDeadConsonants{
    #Declare variables/get parameters
    my @consonantPairs = @{$_[0]};
    my @deadConsonantPairs;
    #Loop through each of the consonantsPairs, adding vowel suppresants to them
    foreach my $consonantPair (@consonantPairs){
        #Extract the consonants from the pair
        my $romanConsonant = $consonantPair->{'roman'};
        my $devaConsonant  = $consonantPair->{'devanagari'};
        #Add the vowel suppressants to them
        my $deadRomanConsonant = $romanConsonant;
        $deadRomanConsonant=~s/a\z//;
        my $deadDevaConsonant = "$devaConsonant\N{DEVANAGARI SIGN VIRAMA}";
        my $deadConsonantPair = {
            'devanagari' => $deadDevaConsonant,
            'roman'      => $deadRomanConsonant,
        }; 
        #Add them to the list of dead consonants
        push @deadConsonantPairs, $deadConsonantPair;
    }
    #return the completed list
    return \@deadConsonantPairs;
}