#!/usr/bin/perl
use warnings;
use strict;
use Fatal;
use utf8;
use charnames ':full';

open my $inFile, "<", "words.txt";
open my $outFile, ">", "words.new.txt";
my @lines;
while (<$inFile>){
    chomp;
    my $line = $_;
    my ($romanWord, $devaWord) = split(/\t/,$line);
    next if $romanWord =~/ai/;
    next if $romanWord =~/au/;
    print $outFile "$line\n";
}
close $inFile;
close $outFile;