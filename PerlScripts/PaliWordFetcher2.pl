#!/usr/bin/perl
use warnings;
use strict;
use Data::Dumper;
use Carp::Assert;
require "PaliWordFetcher2.pm";

#Create two word fetchers
my $romanWordFetcher = PaliWordFetcher2->new();
my $devaWordFetcher = PaliWordFetcher2->new();
#Populate lists of URLs
my @romanUrls;
my @devaUrls;
open my $fhRomn, "<:encoding(UTF-8)", "romanLinks.txt" or die "$!";
open my $fhDeva, "<:encoding(UTF-8)", "devaLinks.txt" or die "$!";
while (<$fhRomn>){
    chomp;
    push (@romanUrls, $_);
}
while (<$fhDeva>){
    chomp;
    push (@devaUrls, $_);
}
close $fhRomn;
close $fhDeva;
#Loop through the urls, collecting all of the words
my %words;
foreach my $index (0..scalar(@devaUrls) -1){
    my @romanWords = $romanWordFetcher->GetUniqueWords($romanUrls[$index]);
    my @devaWords = $devaWordFetcher->GetUniqueWords($devaUrls[$index]);
    next if scalar(@romanWords) != scalar(@devaWords);
    #Put all the words into a hash
    foreach my $index2 (0..scalar(@devaWords)){
        #assert(defined($romanWords[$index2]), $index2);
        if (defined($romanWords[$index2]) && defined($devaWords[$index2])){
            $words{$romanWords[$index2]} = $devaWords[$index2];
        }
    }
}
#Output the contents of the hash to a file
open my $fhWords, ">", "words.txt" or die "$!";
foreach my $key (sort keys %words){
    no warnings;
    my $value = $words{$key};
    print $fhWords "$key\t$value\n";
}
close $fhWords;

#Print out any length differences between the two arrays
#print Dumper(@devaWords[0]);
=validatorOnly
foreach my $index (0 .. scalar(@romanWords)-1) {
    my $numberOfRomans = scalar(@{$romanWords[$index]});
    my $numberOfDevas = scalar(@{$devaWords[$index]});
    if ($numberOfRomans != $numberOfDevas){
        print "$numberOfRomans ROMAN at INDEX $index:\n";
        print Dumper($romanWords[$index]);
        print "\n\n";
        print "$numberOfDevas DEVA at INDEX $index:\n";
        print Dumper($devaWords[$index]);
        print "\n\n\n";
    }
}
=cut
my $x = 1;