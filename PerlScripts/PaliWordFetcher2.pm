#!/usr/bin/perl

use MooseX::Declare;

class PaliWordFetcher2{
    use warnings;
    use strict;
    use LWP::UserAgent;
    use Carp::Assert;
    use HTML::TokeParser;
    use List::MoreUtils qw(uniq);
    use utf8;
    use Encode;
    use Carp;
    
    our $ua;
    our $puncuation = qr/[,–‘"’'?\/.;+\-*=&@!#$%\^\[\[|}{><`~]+/x;
    
    sub BUILD () {
        #Construct the userAgent and configure the proxy
        $ua=LWP::UserAgent->new();
        $ua->proxy(['http','ftp'], 'http://pecsip:G0ATph33r@10.0.0.21:3128');
    }
    
    method GetUniqueWords(Str $url){
        #Attempt to get the content from $url
        my $response = $ua->get($url);
        $response->is_success or confess "Response failed for $url";
        #If sucessful parse the content
        my $content = $response->decoded_content();
        my $htmlParser = HTML::TokeParser->new(doc => \$content);
        #Parse the words out of the content
        my @totalWords;
        my $count = 0;
        while (my $token = $htmlParser->get_tag("p")) {
              my $text = $htmlParser->get_trimmed_text("/p");
              #Filter out unwanted Devanagari characters and other stuff
              ##Filter out stop characters
              #$text=~s/॥//g;
              #$text=~s/।//g;
              ##Filter out abbreviation characters
              #$text=~s/॰//g;
              ##Filter out anything in parenthesis
              #$text=~s/[(].+[)]//g;
              ##Filter out any parenthesis
              #$text=~s/[()]//g;
              ##Filters puncuation marks
              #$text=~s/$puncuation//gx;
              ##trim numbers
              #$text=~s/\d//g;
              ##trim leading and trailing white space
              #$text=~s/\A[ ]+//g;
              #$text=~s/[ ]+\z//g;
              #Pull the words out of the text, filtering repeats
              my @paragraphWords = split(/[ ]+/, $text);
              @paragraphWords = map {$_ =~s/\W|\s//g; $_} @paragraphWords;
              @paragraphWords = grep {defined($_) and $_ !~/\A\s+\z/ and length($_) != 0} @paragraphWords;
              #confess "No words found" if scalar(@paragraphWords) == 0;
              #remove any whitespace words
              push @totalWords, @paragraphWords;
              $count++;
        }
        #Return
        return @totalWords;
    }
}

    
