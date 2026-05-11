package com.vulnwatch.worker.config;

import com.maxmind.db.CHMCache;
import com.maxmind.geoip2.DatabaseReader;
import jakarta.annotation.PostConstruct;
import jakarta.annotation.PreDestroy;
import org.springframework.context.annotation.Configuration;

import java.io.File;
import java.io.IOException;
import java.util.concurrent.atomic.AtomicReference;

@Configuration
public class GeoIpManager {

    private final AtomicReference<DatabaseReader> readerRef =
            new AtomicReference<>();

    @PostConstruct
    public void init() throws IOException {
        reloadDatabase();
    }

    public DatabaseReader reader() {
        return readerRef.get();
    }

    public void reloadDatabase() throws IOException {

        File dbFile = new File("/Users/mitchelntuen/Downloads/GeoLite2-ASN_20260511/GeoLite2-ASN.mmdb");

        DatabaseReader newReader =
                new DatabaseReader.Builder(dbFile)
                        .withCache(new CHMCache())
                        .build();

        DatabaseReader oldReader = readerRef.getAndSet(newReader);

        if (oldReader != null) {
            oldReader.close();
        }
    }

    @PreDestroy
    public void shutdown() {
        DatabaseReader reader = readerRef.getAndSet(null);
        if (reader != null) {
            try {
                reader.close();
            } catch (IOException ignored) {}
        }
    }
}
