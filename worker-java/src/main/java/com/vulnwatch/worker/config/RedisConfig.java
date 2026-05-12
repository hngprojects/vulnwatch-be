package com.vulnwatch.worker.config;

import com.vulnwatch.worker.queue.RedisJobConsumer;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.redis.connection.RedisConnectionFactory;
import org.springframework.data.redis.listener.ChannelTopic;
import org.springframework.data.redis.listener.RedisMessageListenerContainer;

@Configuration
public class RedisConfig {

    public static final String SCAN_JOB_CHANNEL = "scan-jobs";
    public static final String SCAN_RESULTS_QUEUE = "scan-results";

    private final RedisConnectionFactory connectionFactory;

    @Bean
    public ObjectMapper objectMapper() {
        ObjectMapper mapper = new ObjectMapper();
        mapper.registerModule(new JavaTimeModule());
        return mapper;
    }

    @Bean
    public RedisTemplate<String, Object> redisTemplate() {
        RedisTemplate<String, Object> template = new RedisTemplate<>();
        template.setConnectionFactory(connectionFactory);
        template.setKeySerializer(new StringRedisSerializer());
        template.setValueSerializer(new Jackson2JsonRedisSerializer<>(Object.class));
        template.setHashKeySerializer(new StringRedisSerializer());
        template.setHashValueSerializer(new Jackson2JsonRedisSerializer<>(Object.class));
        template.afterPropertiesSet();
        return template;
    }

    @Bean
    public RedisMessageListenerContainer redisContainer(RedisJobConsumer consumer) {
        RedisMessageListenerContainer container = new RedisMessageListenerContainer();
        container.setConnectionFactory(connectionFactory);
        container.addMessageListener(consumer, new ChannelTopic(SCAN_JOB_CHANNEL));
        log.info("Redis listener started on channel: {}", SCAN_JOB_CHANNEL);
        return container;
    }
}
