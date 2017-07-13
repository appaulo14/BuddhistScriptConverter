#!/usr/bin/perl
use MooseX::Declare;

class PaliWordFetcher {
    use warnings;
    use strict;
    use LWP::UserAgent;
    use Carp::Assert;
    use HTML::TokeParser;
    use List::MoreUtils qw(uniq);
    
    my @devanagariWords, @romanWords, @brahmiWords, @kharosthiWords;
    
    method FetchWords(Str :$devanagariUrl?, Str :$romanUrl?, 
            Str :$brahmiUrl?, Str :$kharosthiUrl?){
        #Create and configure our LWP::UserAgent for fetching
        my $ua = LWP::UserAgent->new();
        $ua->proxy(['http','ftp'], 'http://pecsip:G0ATph33r@10.0.0.21:3128');
        #Fetch the data
        if ($devanagariUrl){
            my $content = $ua->get($devanagariUrl);
            my $htmlParser = HTML::TokeParser->new(doc => \$content);
            while (my $token = $htmlParser->get_tag("p")) {
                my $paragraph = $htmlParser->get_trimmed_text("/p");
                my @words = uniq(split(/[]+/,$paragraph));
                push(@devanagariWords,@words);
            }
        }
    }
    method WriteWordsToXMLFile (Str $filename) {
        #Instantiate XML parser
        #Open file and loop through the two word arrays,
        #   writing them to the file
    }
    method ClearWords(){
    }
}
