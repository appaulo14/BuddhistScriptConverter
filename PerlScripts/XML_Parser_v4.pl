#!/usr/bin/perl
use warnings;
use strict;
use XML::Simple;
use Data::Dumper;
use charnames ':full';
use utf8;

#Declare some global variables
my $xmlPathName = "../BuddhistScriptConverter/XML";
my %vowelSigns = (
    'ā' => "\N{DEVANAGARI VOWEL SIGN AA}",
    'i' => "\N{DEVANAGARI VOWEL SIGN I}", 
    'ī' => "\N{DEVANAGARI VOWEL SIGN II}",
    'u' => "\N{DEVANAGARI VOWEL SIGN U}",
    'ū' => "\N{DEVANAGARI VOWEL SIGN UU}",
    'e' => "\N{DEVANAGARI VOWEL SIGN E}",
    'ai'=> "\N{DEVANAGARI VOWEL SIGN AI}",
    'o' => "\N{DEVANAGARI VOWEL SIGN O}",
    'au'=> "\N{DEVANAGARI VOWEL SIGN AU}",
    'ṛ' => "\N{DEVANAGARI VOWEL SIGN VOCALIC R}",
);
my @romanVowels = qw(a ā i ī u ū e o);
#Create xml parser object
my $xmlReader = XML::Simple->new();
#read the xml file
my $devaData = $xmlReader->XMLin("$xmlPathName/DevanagariPaliScript.xml");
#print Dumper($data);
my $newData;
#Define new a new data structure
##Define the diferrent sections
my @irregulars;
push @irregulars, {'roman' => "ṃ", 'devanagari' => "\N{DEVANAGARI SIGN ANUSVARA}"};
push @irregulars, {'roman' => 'ḥ', 'devanagari' => "\N{DEVANAGARI SIGN VISARGA}"};
my @vowels = XML_To_Array($devaData->{'vowels'}->{'element'});
###Put the matras ending a in a separate place with the non-a endings
my @refs = Matra_XML_To_Array($devaData->{'dependentVowelForms'}->{'element'});
my @monoMatras = @{$refs[0]};
my @diMatras   = @{$refs[1]};
my @monoConsonants = @{$refs[2]};
my @diConsonants = @{$refs[3]};
my @monoDeadConsonants = @{$refs[4]};
my @diDeadConsonants   = @{$refs[5]};
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
open my $file, ">", "ScriptDefinitions4.xml" or die;
print $file "$xmlOutput";
close $file;

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
    #Declare some variables
    my @monoMatras;
    my @diMatras;
    my @monoConsonants;
    my @diConsonants;
    my @monoDeadConsonants;
    my @diDeadConsonants;
    #loop through the XML structure, pulling out the data and putting it in 
    #the new, correct format
    foreach my $devaSymbol (@{$_[0]}){
        my $romanForm = $devaSymbol->{'romanForm'};
        my $devaForm = $devaSymbol->{'content'};
        #skip all dependent vowel forms
        next if $romanForm !~ /a\z/;
         #Place the symbol based on whether it's consonant component
         #contains one or two characters
         if (length($romanForm) == 3){
            push @diConsonants, {'roman' => $romanForm, 'devanagari' => $devaForm};
            #Lop off the trailing a to make the dependent vowel forms and dead consonants
            $romanForm =~ s/a\z//;
            push @diDeadConsonants, {
                'roman' => $romanForm,
                'devanagari' => "$devaForm\N{DEVANAGARI SIGN VIRAMA}"
            };
            foreach my $romanVowel (keys %vowelSigns){
                push @diMatras, {
                    'roman' => "$romanForm$romanVowel",
                    'devanagari' => "$devaForm$vowelSigns{$romanVowel}"
                };
            }
         }
         else{ 
            push @monoConsonants, {'roman' => $romanForm, 'devanagari' => $devaForm};
            #Lop off the trailing a to make the dependent vowel forms and dead consonants
            $romanForm =~ s/a\z//;
            push @monoDeadConsonants, {
                'roman' => $romanForm,
                'devanagari' => "$devaForm\N{DEVANAGARI SIGN VIRAMA}"
            };
            foreach my $romanVowel (keys %vowelSigns){
                push @monoMatras, {
                    'roman' => "$romanForm$romanVowel",
                    'devanagari' => "$devaForm$vowelSigns{$romanVowel}"
                };
            }
         }
    }
    #return the array
    return (\@monoMatras,\@diMatras,\@monoConsonants,
        \@diConsonants,\@monoDeadConsonants,\@diDeadConsonants);
}