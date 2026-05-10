package com.vulnwatch.worker.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Primary;

import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

@Configuration
public class AsyncConfig {

    @Primary
    @Bean
    public Executor taskExecutor(){
        return Executors.newFixedThreadPool(10);
    }
}
