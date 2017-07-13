#!/usr/bin/perl
package LWP::ProxiedUserAgent;
use base LWP::UserAgent;
    
    sub new  {
        my $class = shift;
        my $self = $class->SUPER::new();
        #Construct the userAgent and configure the proxy
        $self=LWP::UserAgent->new();
        $self->proxy(['http','ftp'], 'http://pecsip:G0ATph33r@10.0.0.21:3128');
        return $self;
    }
1;