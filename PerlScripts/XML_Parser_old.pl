#!/usr/bin/perl
use warnings;
use strict;
use XML::Simple;
use Data::Dumper;



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

my $xmlPathName = "../BuddhistScriptConverter/XML";
#Create xml parser object
my $xmlReader = XML::Simple->new();
#read the xml file
my $devaData = $xmlReader->XMLin("$xmlPathName/DevanagariPaliScript.xml");
my $romanData = $xmlReader->XMLin("$xmlPathName/RomanPaliScript.xml");
#print Dumper($data);
my $newData;
#Define new a new data structure to output
##Define the diferrent sections
my @irregulars = XML_To_Array($devaData->{'irregulars'}->{'element'});
my @conjuncts = XML_To_Array($devaData->{'conjuncts'}->{'element'});
my @refs = Matra_XML_To_Array($devaData->{'dependentVowelForms'}->{'element'});
my @matras = @{$refs[0]};
my @consonants = @{$refs[1]};
my @vowels = XML_To_Array($devaData->{'vowels'}->{'element'});
my @numbers = XML_To_Array($devaData->{'numbers'}->{'element'});
##Put them all together into one big array
my %outputArray = (
    'irregulars' => {'symbol' => \@irregulars},
    'conjuncts'  => {'symbol' => \@conjuncts},
    'matras'     => {'symbol' => \@matras},
    'consonants' => {'symbol' => \@consonants},
    'vowels'     => {'symbol' => \@vowels},
    'numbers'    => {'symbol' => \@numbers},
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