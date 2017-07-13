#!/usr/bin/perl
use warnings;
use strict;
use Data::Dumper;
use Carp::Assert;
use Moose::Autobox;
use Fatal;
require "PaliWordFetcher2.pm";

#Create two word fetchers
my $romanWordFetcher = PaliWordFetcher2->new();
my $khmerWordFetcher = PaliWordFetcher2->new();
#Populate lists of URLs
my @romanUrls;
my @khmerUrls;
open my $fhRomn, "<:encoding(UTF-8)", "romanLinks.txt";
open my $fhKhmer, "<:encoding(UTF-8)", "khmerLinks.txt";
while (<$fhRomn>){
    chomp;
    push (@romanUrls, $_);
}
while (<$fhKhmer>){
    chomp;
    push (@khmerUrls, $_);
}
close $fhRomn;
close $fhKhmer;
#Assert the correct number of urls
assert(@romanUrls->length() == @khmerUrls->length());
#Loop through the urls, collecting all of the words
my %words;
foreach my $index (0..@khmerUrls->length() -1 ){
    my @romanWords = $romanWordFetcher->GetUniqueWords($romanUrls[$index]);
    my @khmerWords = $khmerWordFetcher->GetUniqueWords($khmerUrls[$index]);
    if (@romanWords->length() != @khmerWords->length()){
        print "skipping\n";
    }
    #Put all the words into a hash
    foreach my $index2 (0..@khmerWords->length() -1){
        #assert(defined($romanWords[$index2]), $index2);
        $words{$romanWords[$index2]} = $khmerWords[$index2];
    }
}
#Output the contents of the hash to a file
open my $fhWords, ">:encoding(UTF-8)", "wordsKhmer.txt";
my @x = %words->keys()->sort();
foreach my $key (sort keys %words){
    my $value = $words{$key};
    print $fhWords "$key\t$value\n";
}
close $fhWords;

#Print out any length differences between the two arrays
#print Dumper(@khmerWords[0]);
=validatorOnly
foreach my $index (0 .. scalar(@romanWords)-1) {
    my $numberOfRomans = scalar(@{$romanWords[$index]});
    my $numberOfDevas = scalar(@{$khmerWords[$index]});
    if ($numberOfRomans != $numberOfDevas){
        print "$numberOfRomans ROMAN at INDEX $index:\n";
        print Dumper($romanWords[$index]);
        print "\n\n";
        print "$numberOfDevas DEVA at INDEX $index:\n";
        print Dumper($khmerWords[$index]);
        print "\n\n\n";
    }
}
=cut
my $x = 1;